using Microsoft.VisualStudio.TestTools.UnitTesting;
using AE.News.Service;
using AE.News.Entity;
using System.Collections.Generic;

namespace AE.Test
{
    [TestClass]
    public class News_NewsService
    {
        #region testdata
        private Feed TestFeed
        {
            get
            {
                return new Feed { Url = "http://yle.fi/uutiset/rss/paauutiset.rss" };
            }
        }

        private Feed TestFeedInvalid
        {
            get
            {
                return new Feed { Url = "http://yle.fi/uutiset/rss/somethingthatdoesnotexist.rss" };
            }
        }

        private List<Feed> TestFeedList
        {
            get
            {
                return new List<Feed>
                {
                    new Feed { Url = "http://yle.fi/uutiset/rss/paauutiset.rss" },
                    new Feed { Url = "http://yle.fi/uutiset/rss/uutiset.rss" },
                    new Feed { Url = "http://yle.fi/uutiset/rss/luetuimmat.rss" },
                    new Feed { Url = "http://yle.fi/uutiset/rss/uutiset.rss?osasto=kotimaa" },
                    new Feed { Url = "http://yle.fi/uutiset/rss/uutiset.rss?osasto=ulkomaat" },
                    new Feed { Url = "http://yle.fi/uutiset/rss/uutiset.rss?osasto=talous" },
                    new Feed { Url = "http://yle.fi/uutiset/rss/uutiset.rss?osasto=politiikka" },
                    new Feed { Url = "http://yle.fi/uutiset/rss/uutiset.rss?osasto=kulttuuri" },
                    new Feed { Url = "http://yle.fi/uutiset/rss/uutiset.rss?osasto=viihde" },
                    new Feed { Url = "http://yle.fi/uutiset/rss/uutiset.rss?osasto=tiede" }
                };
            }
        }

        private List<Feed> TestFeedListInvalid
        {
            get
            {
                return new List<Feed>
                {
                    new Feed { Url = "http://yleasdfsfadf.fi/uutiset/rss/paauutiset.rss" },
                    new Feed { Url = "http://yle.fi/uutiset/rss/somethingthatdoesnotexist.rss" },
                    new Feed { Url = "http://yle.fi/uutiset/rss/luetuimmat.rssadfsfdasdfadfasdfsdf" },
                    new Feed { Url = "http://yle.fi/uutiset/rss/somethingthatdoesnotexist.rss?osasto=talous" },
                    new Feed { Url = "http://somethingthatdoesnotexist.fi/uutiset/rss/uutiset.rss?osasto=viihde" },
                    new Feed { Url = "htadfasfdsdftp://yle.fi/uutiset/rss/uutiset.rss?osasto=tiede" }
                };
            }
        }
        #endregion

        [TestMethod]
        public void FetchFeed()
        {
            NewsService ns = NewsService.Instance;
            List<Article> list = ns.fetchFeed(TestFeed).Result;
            Assert.AreNotEqual(0, list.Count);
        }

        [TestMethod]
        public void FetchFeedInvalid()
        {
            NewsService ns = NewsService.Instance;
            List<Article> list = ns.fetchFeed(TestFeedInvalid).Result;
            Assert.AreEqual(0, list.Count);
        }


        [TestMethod]
        public void FetchFeeds()
        {
            NewsService ns = NewsService.Instance;
            List<Article> list = ns.fetchFeeds(TestFeedList);
            Assert.AreNotEqual(0, list.Count);
        }

        [TestMethod]
        public void FetchFeedsInvalid()
        {
            NewsService ns = NewsService.Instance;
            List<Article> list = ns.fetchFeeds(TestFeedListInvalid);
            Assert.AreEqual(0, list.Count);
        }
    }
}
