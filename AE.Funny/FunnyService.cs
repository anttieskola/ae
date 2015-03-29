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
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AE.Funny
{
    /// <summary>
    /// Service to gather together funnyposts into DB.
    /// 
    /// Note, usage of paraller threads and fancy stuff does not really
    /// bring much speed benefits but it is cool and i am in charge.
    /// </summary>
    internal class FunnyService
    {
        #region fields
        private const int MAX_TASKS = 10;
        private const int MAX_COMMENTS = 3;
        private const string FUNNY_URL = "http://www.reddit.com/r/funny.json?sort=hot";
        #endregion

        #region properties
        private int _pageCount;
        /// <summary>
        /// Page count how many reddit funny pages we scrape, each page has 25 posts
        /// Value must be >=1
        /// </summary>
        /// <returns></returns>
        public int PageCount
        {
            get { return _pageCount; }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException("PageCount", "Value can be only >= 1");
                }
                _pageCount = value;
            }
        }
        #endregion
        public FunnyService()
        {
            _pageCount = 4;
        }

        internal async Task<List<FunnyPost>> fetchPosts()
        {
            // Todo: if page parameter someday works could be made faster as no need for after token
            // scrape pages, can't do in paraller as require the after token to get next page
            List<RedditPost> rpList = new List<RedditPost>();
            string after = "";
            for (int currentPage = 1; currentPage <= _pageCount; currentPage++)
            {
                string pageUrl = FUNNY_URL + after;
                try
                {
                    RedditPostPage rpp = await RedditScraper.ParsePosts(pageUrl);
                    after = "&after=" + rpp.After; // set after to get next page of posts
                    rpList.AddRange(rpp.Posts);
                }
                catch (WebException we)
                {
                    Debug.WriteLine(we.Message);
                    return null;
                }
            }

            // convert reddit posts into funnyposts
            ConcurrentStack<FunnyPost> fpStack = new ConcurrentStack<FunnyPost>();
            int dclBackup = ServicePointManager.DefaultConnectionLimit;
            ServicePointManager.DefaultConnectionLimit = MAX_TASKS;
            Parallel.ForEach(rpList, new ParallelOptions { MaxDegreeOfParallelism = MAX_TASKS}, post =>
            {
                FunnyPost fp = createFunnyPost(post).Result;
                if (fp != null)
                {
                    fpStack.Push(fp);
                }
            });
            ServicePointManager.DefaultConnectionLimit = dclBackup;
            return new List<FunnyPost>(fpStack);
        }

        internal async Task<List<FunnyPost>> fetchPostsMT()
        {
            // Todo: if page parameter someday works could be made faster as no need for after token
            // scrape pages, can't do in paraller as require the after token to get next page
            List<RedditPost> rpList = new List<RedditPost>();
            string after = "";
            for (int currentPage = 1; currentPage <= _pageCount; currentPage++)
            {
                string pageUrl = FUNNY_URL + after;
                try
                {
                    RedditPostPage rpp = await RedditScraper.ParsePosts(pageUrl);
                    after = "&after=" + rpp.After; // set after to get next page of posts
                    rpList.AddRange(rpp.Posts);
                }
                catch (WebException we)
                {
                    Debug.WriteLine(we.Message);
                    return null;
                }
            }

            // convert reddit posts into funnyposts
            ConcurrentStack<FunnyPost> fpStack = new ConcurrentStack<FunnyPost>();
            int dclBackup = ServicePointManager.DefaultConnectionLimit;
            ServicePointManager.DefaultConnectionLimit = MAX_TASKS;
            Task[] cfpTasks = new Task[MAX_TASKS];
            for (int rpIndex = 0; rpIndex < rpList.Count; /* note, no increment here*/)
            {
                // launch tasks
                for (int taskIndex = 0; taskIndex < MAX_TASKS; taskIndex++)
                {
                    // post is passed as parameter for task
                    cfpTasks[taskIndex] = Task.Factory.StartNew((rp) =>
                    {
                        FunnyPost fp = createFunnyPost((RedditPost)rp).Result;
                        if (fp != null)
                        {
                            fpStack.Push(fp);
                        }
                    }, rpList[rpIndex]);
                    // increment post array index
                    rpIndex++; 
                    // break if all posts are processing
                    if (rpIndex >= rpList.Count)
                    {
                        break;
                    }
                }
                Task.WaitAll(cfpTasks); // wait for tasks to complete
            }
            ServicePointManager.DefaultConnectionLimit = dclBackup;
            return new List<FunnyPost>(fpStack);
        }

        internal async Task<List<FunnyPost>> fetchPostsST()
        {
            // Todo: if page argument someday works could be made faster
            // scrape pages, can't do in paraller as require the after token to get next page
            List<RedditPost> rpList = new List<RedditPost>();
            string after = "";
            for (int currentPage = 1; currentPage <= _pageCount; currentPage++)
            {
                string pageUrl = FUNNY_URL + after;
                try
                {
                    RedditPostPage rpp = await RedditScraper.ParsePosts(pageUrl);
                    after = "&after=" + rpp.After; // set after to get next page of posts
                    rpList.AddRange(rpp.Posts);
                }
                catch (WebException we)
                {
                    Debug.WriteLine(we.Message);
                    return null;
                }
            }

            // convert reddit posts into funnyposts
            List<FunnyPost> fpList = new List<FunnyPost>();
            foreach (RedditPost rp in rpList)
            {
                FunnyPost fp = createFunnyPost(rp).Result;
                if (fp != null)
                {
                    fpList.Add(fp);
                }
            }
            return fpList;
        }

        internal static async Task<FunnyPost> createFunnyPost(RedditPost rp)
        {
            // funny  post
            FunnyPost fp = new FunnyPost { Title = rp.Title, RedditId = rp.Id };

            // get direct link to picture
            if (!IsPictureLink(rp.Url))
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
                fp.Comments = await RedditScraper.ParseComments(fp.RedditId, 3);
            } catch (WebException we)
            {
                // comments are not crucial for post
                Debug.WriteLine(we.Message);
                fp.Comments = null;
            }
            return fp;
        }

        #region helpers
        internal static bool IsPictureLink(string url)
        {
            return Regex.IsMatch(url.ToLower(), @"(\.(jpg|jpeg|png|gif|gifv)$)");
        }
        #endregion
    }
}
