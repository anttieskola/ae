using AE.Reddit.Entity;
using AE.Reddit.Utils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
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
        private int _pageCount;
        /// <summary>
        /// Page count how many reddit funny pages we scrape, each page has around 25 posts
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

        public FunnyService()
        {
            _pageCount = 4;
        }

        internal int Maintenance()
        {
            //ServicePointManager.DefaultConnectionLimit = PageCount;

            // acquire posts from reddit
            ConcurrentStack<RedditPost> csPosts = new ConcurrentStack<RedditPost>();
            string rfUrl = "http://www.reddit.com/r/funny.json?sort=hot&page=";
            Task[] ppTasks = new Task[PageCount];
            for (int i = 0; i < PageCount; i++)
            {
                ppTasks[i] = Task.Factory.StartNew(
                    (page) =>
                    {
                        List<RedditPost> l = RedditScraper.ParsePosts(rfUrl + page).Result;
                        csPosts.PushRange(l.ToArray());
                    }, i+1
               );
            }
            Task.WaitAll(ppTasks);

            // convert stack to list and remove possible duplicates
            List<RedditPost> rpList = new List<RedditPost>(csPosts.Distinct());

            return rpList.Count;
        }

        internal int MaintenanceST()
        {
            // acquire posts from reddit
            List<RedditPost> rpList = new List<RedditPost>();
            string rfUrl = "http://www.reddit.com/r/funny.json?sort=hot&page=";
            Task[] ppTasks = new Task[PageCount];
            for (int i = 0; i < PageCount; i++)
            {
                List<RedditPost> l = RedditScraper.ParsePosts(rfUrl + i).Result;
                rpList.AddRange(l);
            }

            rpList = rpList.Distinct().ToList();
            return rpList.Count;
        }

    }
}
