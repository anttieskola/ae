using AE.News.Abstract;
using AE.News.Entity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AE.News
{
    public sealed class NewsContext : INewsRepository
    {
        #region fields
        private const int maxArticleAgeInDays = 30;
        private int _lastId;
        private List<IFeed> _feeds;
        private List<NewsArticle> _db;
        private List<string> _tags;
        #endregion

        #region singleton pattern
        // Note: http://csharpindepth.com/Articles/General/Singleton.aspx
        // Note: http://blog.stephencleary.com/2012/08/asynchronous-lazy-initialization.html
        // Note: http://stackoverflow.com/questions/12534705/singleton-class-which-requires-some-async-call
        private static readonly AsyncLazy<NewsContext> lazy =
            new AsyncLazy<NewsContext>(Setup);

        /// <summary>
        /// instance
        /// </summary>
        public static AsyncLazy<NewsContext> Instance
        {
            get
            {
                return lazy;
            }
        }

        /// <summary>
        /// instance
        /// </summary>
        /// <returns></returns>
        public static async Task<NewsContext> GetInstance()
        {
            return await lazy;
        }

        /// <summary>
        /// async constructor called by lazy
        /// </summary>
        /// <returns></returns>
        private static async Task<NewsContext> Setup()
        {
            NewsContext nc = new NewsContext();
            await nc.Maintenance();
            return nc;
        }

        /// <summary>
        /// private synchronized constructor
        /// </summary>
        private NewsContext()
        {
            // db
            _tags = new List<string>();
            _db = new List<NewsArticle>();

            // feed(s) setup
            _lastId = 0;
            _feeds = new List<IFeed>();
            _feeds.Add(new YleFeeds());
        }
        #endregion

        #region maintenance job
        /// <summary>
        /// Maintain news DB
        /// </summary>
        /// <returns></returns>
        public async Task Maintenance()
        {
            // fetch new
            foreach (var feed in _feeds)
            {
                foreach (var article in await feed.Fetch())
                {
                    if (!_db.Any(a => a.Hash == article.Hash))
                    {
                        Debug.WriteLine("NewsContext - Maintenance - New article, hash:{0}, title:{1}", article.Hash, article.Title);
                        // add to db
                        article.Id = _lastId;
                        _lastId++;
                        _db.Add(article);

                        // pickup tag to list if new
                        foreach (var tag in article.Tag)
                        {
                            if (!_tags.Contains(tag))
                            {
                                _tags.Add(tag);
                            }
                        }
                    }
                }
            }
            // clean old
            List<int> oldOnes = new List<int>();
            foreach (var article in _db)
            {
                TimeSpan val = article.Date.Subtract(DateTime.Now);
                int totalDaysOld = (int)(val.TotalDays * -1.0);
                if (totalDaysOld > maxArticleAgeInDays)
                {
                    Debug.WriteLine("NewsContext - Maintenance - Removing article, hash:{0}, id:{1}, title:{2}, ageInDays:{3}", article.Hash, article.Id, article.Title, totalDaysOld);
                    oldOnes.Add(article.Id);
                }
            }
            if (oldOnes.Count > 0)
            {
                _db.RemoveAll(a => oldOnes.Contains(a.Id));
            }
            Debug.WriteLine("NewsContext - Maintenance - Complete - ArticleCount: {0}", _db.Count());
        }
        #endregion

        #region repository api
        public IEnumerable<NewsArticle> Get(Func<NewsArticle, bool> filter = null)
        {
            List<NewsArticle> articles;
            if (filter != null)
            {
                articles = _db.Where(filter).OrderByDescending(a => a.Date).ToList();
            }
            else
            {
                articles = _db.OrderByDescending(a => a.Date).ToList();
            }
            return articles;
        }

        public IEnumerable<NewsArticle> Get(string tag)
        {
            var articles = from a in _db
                           where a.Tag.Contains(tag)
                           orderby a.Date descending
                           select a;
            return articles.ToList();
        }

        public NewsArticle Get(int id)
        {
            return _db.First(a => a.Id == id);
        }

        public IEnumerable<string> Tags()
        {
            return _tags;
        }
        #endregion

    }

    #region lazy constructor
    /// <summary>
    /// Lazy object with asynchronous constructor
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class AsyncLazy<T> : Lazy<Task<T>>
    {
        public AsyncLazy(Func<Task<T>> taskFactory) :
            base(() => Task.Factory.StartNew(() => taskFactory()).Unwrap())
        {

        }
        public TaskAwaiter<T> GetAwaiter()
        {
            return Value.GetAwaiter();
        }
    }
    #endregion
}


