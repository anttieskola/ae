using AE.EF.Abstract;
using AE.Funny.Dal;
using AE.Funny.Entity;
using AE.Imgur.Utils;
using AE.Reddit.Entity;
using AE.Reddit.Utils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AE.Funny.Service
{
    /// <summary>
    /// Service to gather together funnyposts into DB.
    /// 
    /// Note, usage of paraller threads and fancy stuff does not really
    /// bring much speed benefits but it is cool and i am in charge.
    /// </summary>
    public class FunnyService
    {
        #region fields
        private const int POST_FETCH_LIMIT = 100;
        private const int MAX_TASKS = 10;
        private const int MAX_COMMENTS = 3;
        private const int POST_KEEP_COUNT = 500;
        private const string FUNNY_URL = "http://www.reddit.com/r/funny.json?sort=hot";
        private static readonly DateTime UNIX_START = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private volatile bool _running;
        private FunnyRepository _repo;
        #endregion

        #region singleton
        private static readonly Lazy<FunnyService> lazy = new Lazy<FunnyService>(
            () => new FunnyService());
        /// <summary>
        /// Acquire instance of the service
        /// </summary>
        /// <returns></returns>
        public static FunnyService Instance
        {
            get
            {
                return lazy.Value;
            }
        }
        private FunnyService()
        {
            _repo = new FunnyRepository();
        }
        #endregion

        /// <summary>
        /// When maintenance was last ran successfully (UtcTime).
        /// Note! Will be DateTime.MinValue if never ran.
        /// </summary>
        /// <returns>UtcTime</returns>
        public DateTime LastSuccess()
        {
            DateTime lastSuccess = (from fm in _repo.Query<Maintenance>()
                                    where fm.Success == true
                                    orderby fm.EndTime descending
                                    select fm.EndTime).FirstOrDefault();
            return lastSuccess;
        }

        /// <summary>
        /// Maintenance run, fetches new posts and manages repository.
        /// Might fail miserably.
        /// </summary>
        public async Task RunAsync()
        {
            Trace.WriteLine("FunnyService, RunAsync");
            if (!_running)
            {
                Trace.WriteLine("FunnyService, start");
                _running = true;
                Maintenance m = new Maintenance { StartTime = DateTime.UtcNow, EndTime = DateTime.MaxValue, Success = false };
                _repo.Insert<Maintenance>(m);
                await _repo.CommitAsync();
                try
                {
                    m.Inserted = await insert();
                    Trace.WriteLine("FunnyService, insert:" + m.Inserted.ToString());
                    if (m.Inserted > 0)
                    {
                        m.Deleted = await delete();
                        Trace.WriteLine("FunnyService, delete:" + m.Deleted.ToString());
                    }
                    m.Success = true;
                }
                catch (Exception e)
                {
                    Trace.WriteLine("FunnyService, exception:" + e.Message);
                    m.Exception = e.Message;
                }
                m.EndTime = DateTime.UtcNow;
                _repo.Update<Maintenance>(m);
                await _repo.CommitAsync();
                _running = false;
                Trace.WriteLine("FunnyService, end");
            }
        }

        /// <summary>
        /// Fetch latest posts and insert into repository if they are new
        /// </summary>
        /// <returns>count of new posts or -1 in case or error</returns>
        internal async Task<int> insert()
        {
            List<Post> listFp = await fetchPosts();
            int inserted = -1;
            // check if fetch successful
            if (listFp != null)
            {
                inserted = 0;
                foreach (Post post in listFp)
                {
                    // is it new?
                    if (!_repo.Query<Post>().Any(p => p.RedditId == post.RedditId))
                    {
                        _repo.Insert<Post>(post);
                        inserted++;
                    }
                }
                await _repo.CommitAsync();
                return inserted;
            }
            return inserted;
        }

        /// <summary>
        /// Cleanup repository from old posts
        /// </summary>
        /// <returns>number of posts removed</returns>
        internal async Task<int> delete()
        {
            // how many posts we have in repository?
            int removeCount = _repo.Query<Post>().Count();
            removeCount -= POST_KEEP_COUNT;

            // remove excess
            if (removeCount > 0)
            {
                int toRemove = removeCount;
                // fetch id's in order that olders is first
                var postIds = from p in _repo.Query<Post>()
                              orderby p.Created
                              select p.PostId;
                foreach (int id in postIds)
                {
                    // remove post
                    _repo.Delete<Post>(id);
                    toRemove--; // counter decremeant
                    if (toRemove == 0) // check
                    {
                        break;
                    }
                }
                await _repo.CommitAsync();
            }
            else
            {
                removeCount = 0;
            }
            return removeCount;
        }

        /// <summary>
        /// Fetch latest posts
        /// </summary>
        /// <returns>List or null in case of errors</returns>
        internal async Task<List<Post>> fetchPosts()
        {
            // fetch posts (amount of limit set)
            HashSet<RedditPost> rpList;
            try
            {
                RedditPostPage rpp = await RedditScraper.ParsePosts(FUNNY_URL + "&limit=" + POST_FETCH_LIMIT);
                rpList = rpp.Posts;
            }
            catch (WebException we)
            {
                Debug.WriteLine(we.Message);
                return null;
            }

            // convert reddit posts into funnyposts
            ConcurrentStack<Post> fpStack = new ConcurrentStack<Post>();
            int dclBackup = ServicePointManager.DefaultConnectionLimit;
            ServicePointManager.DefaultConnectionLimit = MAX_TASKS;
            Parallel.ForEach(rpList, new ParallelOptions { MaxDegreeOfParallelism = MAX_TASKS }, post =>
            {
                Post fp = createFunnyPost(post).Result;
                if (fp != null)
                {
                    fpStack.Push(fp);
                }
            });
            ServicePointManager.DefaultConnectionLimit = dclBackup;
            return new List<Post>(fpStack);
        }

        /// <summary>
        /// Create funny post from reddit post
        /// </summary>
        /// <param name="rp"></param>
        /// <returns>Post or null in case of error</returns>
        internal static async Task<Post> createFunnyPost(RedditPost rp)
        {
            // nsfw check
            if (rp.Title.IndexOf("nsfw", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return null;
            }

            // funny post
            Post fp = new Post { Title = rp.Title, RedditId = rp.Id };
            
            // timestamp
            fp.Created = UNIX_START.AddSeconds(rp.Created_Utc);

            // get direct link to picture
            fp.ImageUrl = await createPictureLink(rp.Url);
            if (fp.ImageUrl == null)
            {
                // no picture => no post
                return null;
            }
            
            // fetch comments
            try
            {
                List<RedditComment> comments = await RedditScraper.ParseComments(fp.RedditId);
                // take 3 first comments to be added into db
                foreach (RedditComment comment in comments.Take(3))
                {
                    fp.Comments.Add(new Comment { Text = comment.Body });
                }
            }
            catch (WebException we)
            {
                // comments are not crucial, so we just forget them in case of error
                Debug.WriteLine(we.Message);
            }
            return fp;
        }

        #region helpers
        /// <summary>
        /// Create direct picture link from given url
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        internal static async Task<string> createPictureLink(string url)
        {
            // is it already rdy?
            if (isPictureLink(url))
            {
                return url;
            }

            // try to create direct link
            if (url.IndexOf("imgur", StringComparison.InvariantCultureIgnoreCase) != -1)
            {
                // imgur
                try
                {
                    url = await ImgurScraper.GetImageUrl(url);
                }
                catch (WebException we)
                {
                    // imgur scrape failed
                    Debug.WriteLine(we.Message);
                }
            }

            // result
            if (isPictureLink(url))
            {
                return url;
            }
            return null;
        }

        /// <summary>
        /// Check is given url link to picture
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        internal static bool isPictureLink(string url)
        {
            if (url == null)
            {
                return false;
            }
            // Todo: add imgurs gifv when we solve display issues
            return Regex.IsMatch(url.ToLower(), @"(\.(jpg|jpeg|png|gif|gif)$)");
        }
        #endregion
    }
}
