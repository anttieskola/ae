using AE.News.Dal;
using AE.News.Entity;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AE.News.Service
{
    public class NewsService
    {
        #region fields
        private NewsRepository _repo;
        private readonly int MAX_TASKS = 10;
        private readonly int MAX_ARTICLE_AGE_IN_DAYS = 30;
        private volatile bool _running;
        #endregion

        #region singleton
        private static readonly Lazy<NewsService> lazy = new Lazy<NewsService>(
            () => new NewsService());

        /// <summary>
        /// Acquire instance of the service
        /// </summary>
        /// <returns></returns>
        public static NewsService Instance
        {
            get
            {
                return lazy.Value;
            }
        }
        private NewsService()
        {
            _repo = new NewsRepository();
        }
        #endregion

        /// <summary>
        /// When service / maintenance was last ran successfully
        /// Note, if it has never been ran value will be DateTime.MinValue
        /// </summary>
        /// <returns></returns>
        public DateTime LastSuccess()
        {
            DateTime ls = (from m in _repo.Query<Maintenance>()
                           where m.Success == true
                           orderby m.EndTime descending
                           select m.EndTime).FirstOrDefault();
            return ls;
        }

        /// <summary>
        /// Run service / maintenance.
        /// Fetches new posts deletes too old ones
        /// </summary>
        /// <returns></returns>
        public async Task RunAsync()
        {
            Trace.WriteLine("NewsService, RunAsync");
            if (!_running)
            {
                Trace.WriteLine("NewsService, start");
                _running = true;
                Maintenance m = new Maintenance { StartTime = DateTime.UtcNow, EndTime = DateTime.MaxValue, Success = false };
                _repo.Insert<Maintenance>(m);
                await _repo.CommitAsync();
                try
                {
                    m.Inserted = await insert();
                    Trace.WriteLine("NewsService, inserted:" + m.Inserted.ToString());
                    if (m.Inserted > 0)
                    {
                        m.Deleted = await delete();
                        Trace.WriteLine("NewsService, deleted:" + m.Deleted.ToString());
                    }
                    m.Success = true;
                }
                catch (Exception e)
                {
                    Trace.WriteLine("NewsService, exception:" + e.Message);
                    m.Exception = e.Message;
                }
                m.EndTime = DateTime.UtcNow;
                _repo.Update<Maintenance>(m);
                await _repo.CommitAsync();
                _running = false;
                Trace.WriteLine("NewsService, end");
            }
        }

        /// <summary>
        /// Fetches new news articles and adds them into repository
        /// </summary>
        /// <returns>Count of new articles</returns>
        internal async Task<int> insert()
        {
            // counter
            int newArticles = 0;
            // fetch feeds from db
            List<Feed> feeds = _repo.Query<Feed>().ToList();
            // fetch articles from feeds
            List<Article> feedArticles = await Task<List<Article>>.Factory.StartNew(
                (list) => { return fetchFeeds((List<Feed>)list); }, feeds);
            // gather all tags for articles and remove duplicates
            List<Article> articles = new List<Article>();
            foreach (Article a in feedArticles)
            {
                // we gathered tags for this one?
                if (articles.Any(x => x.SourceUrl == a.SourceUrl))
                {
                    continue;
                }
                // gather tags
                foreach (Article at in feedArticles)
                {
                    if (a == at)
                    {
                        continue;
                    }
                    if (a.SourceUrl == at.SourceUrl)
                    {
                        a.Tags.Add(at.Tags.FirstOrDefault());
                    }
                }
                // add to gathered list
                articles.Add(a);
            }
            // add new articles and update tags if missing (failed fetch earlier...)
            foreach (Article article in articles)
            {
                Article dbArticle = _repo.Query<Article>().FirstOrDefault(a => a.SourceUrl == article.SourceUrl);
                if (dbArticle != null)
                {
                    // check that article in db contains atleast the same tags our current fetch got
                    List<Tag> newTags = new List<Tag>(article.Tags.Except(dbArticle.Tags));
                    if (newTags.Count > 0)
                    {
                        // add missing tags
                        foreach (Tag t in newTags)
                        {
                            dbArticle.Tags.Add(t);
                        }
                        _repo.Update<Article>(dbArticle);
                    }
                }
                else
                {
                    // new article
                    _repo.Insert<Article>(article);
                    newArticles++;
                }
            }
            await _repo.CommitAsync();
            return newArticles;
        }

        /// <summary>
        /// Delete articles older than MAX_ARTICLE_AGE_IN_DAYS from repository
        /// </summary>
        /// <returns>Count of deleted articles</returns>
        internal async Task<int> delete()
        {
            // counter
            int deletedPosts = 0;
            // look for too old articles in db
            DateTime dateOld = DateTime.UtcNow.Subtract(new TimeSpan(MAX_ARTICLE_AGE_IN_DAYS, 0, 0, 0));
            // gather id's
            var oldArticleIds = from a in _repo.Query<Article>()
                                where a.Date < dateOld
                                select a.ArticleId;
            // delete
            foreach (var id in oldArticleIds)
            {
                _repo.Delete<Article>(id);
                deletedPosts++;
            }
            // commit
            if (deletedPosts > 0)
            {
                await _repo.CommitAsync();
            }
            return deletedPosts;
        }

        /// <summary>
        /// Fetch articles of given list of feeds
        /// Note, this blocks caller for second or few depending
        /// on the amount of feeds.
        /// </summary>
        /// <param name="feedList"></param>
        /// <returns></returns>
        internal List<Article> fetchFeeds(List<Feed> feedList)
        {
            ConcurrentStack<Article> articleStack = new ConcurrentStack<Article>();
            int dclBackup = ServicePointManager.DefaultConnectionLimit;
            ServicePointManager.DefaultConnectionLimit = MAX_TASKS;
            Parallel.ForEach(feedList, new ParallelOptions { MaxDegreeOfParallelism = MAX_TASKS },
                feed =>
            {
                foreach (Article a in fetchFeed(feed).Result)
                {
                    articleStack.Push(a);
                }
            });
            ServicePointManager.DefaultConnectionLimit = dclBackup;
            return new List<Article>(articleStack);
        }

        /// <summary>
        /// Fetch articles of given feed
        /// </summary>
        /// <param name="feed"></param>
        /// <returns></returns>
        internal async Task<List<Article>> fetchFeed(Feed feed)
        {
            List<Article> news = new List<Article>();
            try
            {
                // get rss feed
                WebRequest req = WebRequest.Create(feed.Url);
                WebResponse response = await req.GetResponseAsync();
                XDocument xml = XDocument.Load(response.GetResponseStream());
                // pickup only item's (news articles)
                var data = from item in xml.Descendants("item")
                           select item;
                // create list of em
                foreach (var d in data)
                {
                    Article a = new Article();
                    // title
                    if (d.Element("title") != null)
                    {
                        a.Title = d.Element("title").Value;
                    }
                    // description
                    if (d.Element("description") != null)
                    {
                        a.Description = d.Element("description").Value;
                    }
                    // date
                    if (d.Element("pubDate") != null)
                    {
                        // <pubDate>Thu, 9 Oct 2014 13:22:17 +0300</pubDate>
                        // http://msdn.microsoft.com/en-us/library/8kb3ddd4(v=vs.110).aspx
                        const string format = "ddd, d MMM yyyy HH:mm:ss zzz";
                        try
                        {
                            a.Date = DateTime.ParseExact(d.Element("pubDate").Value, format, CultureInfo.InvariantCulture);
                        }
                        catch (FormatException e)
                        {
                            Debug.WriteLine(e.Message);
                            a.Date = DateTime.UtcNow;
                        }
                    }
                    else
                    {
                        a.Date = DateTime.UtcNow;
                    }
                    // content
                    if (d.Element(XName.Get("encoded", "http://purl.org/rss/1.0/modules/content/")) != null)
                        a.Content = d.Element(XName.Get("encoded", "http://purl.org/rss/1.0/modules/content/")).Value;
                    // image
                    if (d.Element("enclosure") != null && d.Element("enclosure").HasAttributes)
                    {
                        string imageUrl = (string)d.Element("enclosure").Attribute("url");
                        a.ImageUrl = imageUrl;
                    }

                    // link to whole story
                    if (d.Element("guid") != null)
                    {
                        a.SourceUrl = d.Element("guid").Value;
                    }
                    a.Tags.Add(feed.Tag);
                    news.Add(a);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("YleFeeds - FetchFeed - Fatal exception: {0}", e.Message);
            }
            return news;
        }
    }
}
