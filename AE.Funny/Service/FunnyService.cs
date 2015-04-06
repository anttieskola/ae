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
            if (!_running)
            {
                _running = true;
                Debug.WriteLine("FunnyService, start");
                Maintenance fm = new Maintenance { StartTime = DateTime.UtcNow, Success = false };
                try
                {
                    fm.Inserted = await insert();
                    Debug.WriteLine("FunnyService, insert:{0}", fm.Inserted);
                    if (fm.Inserted > 0)
                    {
                        fm.Deleted = await delete();
                        fm.Success = true;
                        Debug.WriteLine("FunnyService, delete:{0}", fm.Deleted);
                    }
                }
                catch (AggregateException ae)
                {
                    Debug.WriteLine("FunnyService, exception:{0}", ae.Message);
                }
                fm.EndTime = DateTime.UtcNow;
                _repo.Insert<Maintenance>(fm);
                await _repo.CommitAsync();
                Debug.WriteLine("FunnyService, end");
                _running = false;
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
            // funny post
            Post fp = new Post { Title = rp.Title, RedditId = rp.Id };
            
            // timestamp
            DateTime unixStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            fp.Created = unixStart.AddSeconds(rp.Created_Utc);

            // get direct link to picture
            if (!isPictureLink(rp.Url))
            {
                // we can scrape to picture url from imgur
                if (rp.Url.IndexOf("imgur", StringComparison.InvariantCultureIgnoreCase) != -1)
                {
                    try
                    {
                        fp.ImageUrl = await ImgurScraper.GetImageUrl(rp.Url);
                    }
                    catch (WebException we)
                    {
                        Debug.WriteLine(we.Message);
                        return null;
                    }
                    if (fp.ImageUrl == null)
                    {
                        return null;
                    }
                }
            }
            else
            {
                fp.ImageUrl = rp.Url;
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
        /// Check is given url link to picture
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        internal static bool isPictureLink(string url)
        {
            // Todo: add imgurs gifv when we solve display issues
            return Regex.IsMatch(url.ToLower(), @"(\.(jpg|jpeg|png|gif|gif)$)");
        }
        #endregion
    }
}
