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

        #region update job
        /// <summary>
        /// Maintain news DB
        /// </summary>
        /// <returns></returns>
        public async Task Maintenance()
        {
            // clean old
            _db.RemoveAll(a => (a.Date.CompareTo(DateTime.Now.AddDays(maxArticleAgeInDays)) < 0));

            // fetch new
            foreach (var feed in _feeds)
            {
                foreach (var article in await feed.Fetch())
                {
                    if (!_db.Any(a => a.Hash == article.Hash))
                    {
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
            Debug.WriteLine("NewsContext - Maintenance - Complete - ArticleCount: {0}", _db.Count());
        }
        #endregion

        #region repository api
        public IEnumerable<NewsArticle> Get(Func<NewsArticle, bool> filter = null)
        {
            if (filter != null)
            {
                return _db.Where(filter).ToList();
            }
            return _db.ToList();
        }

        public IEnumerable<NewsArticle> Get(string tag)
        {
            return _db.Where(a => a.Tag.Contains(tag)).ToList();
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
}


