using AE.Reddit.Entity;
using AE.Reddit.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace AE.Test
{
    [TestClass]
    public class Reddit_PostScraper
    {
        [TestMethod]
        public void ParsePosts()
        {
            RedditScraper fs = new RedditScraper();
            RedditPostPage rpp = RedditScraper.ParsePosts("http://www.reddit.com/r/csharp.json").Result;
            Assert.IsFalse(rpp.Posts.Count == 0);
        }

        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public void ParsePostsInvalidUri()
        {
            RedditScraper fs = new RedditScraper();
            RedditPostPage rpp = RedditScraper.ParsePosts("http://127.0.0.1.json").Result;
        }
    }
}
