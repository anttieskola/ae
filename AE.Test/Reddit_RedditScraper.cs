using AE.Reddit.Entity;
using AE.Reddit.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace AE.Test
{
    [TestClass]
    public class Reddit_RedditScraper
    {
        [TestMethod]
        public void ParsePosts()
        {
            RedditPostPage rpp = RedditScraper.ParsePosts("http://www.reddit.com/r/csharp.json").Result;
            Assert.IsFalse(rpp.Posts.Count == 0);
        }

        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public void ParsePostsInvalidUri()
        {
            RedditPostPage rpp = RedditScraper.ParsePosts("http://127.0.0.1.json").Result;
        }

        [TestMethod]
        public void ParseComments()
        {
            List<RedditComment> lrc = RedditScraper.ParseComments("31ho69").Result;
            //List<RedditComment> lrc = RedditScraper.ParseComments("31ixrv").Result;
            //List<RedditComment> lrc = RedditScraper.ParseComments("31iw5k").Result;
        }
    }
}
