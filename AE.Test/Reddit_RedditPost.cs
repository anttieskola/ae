using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AE.Reddit.Entity;
using System.Collections.Generic;

namespace AE.Test
{
    [TestClass]
    public class Reddit_RedditPost
    {
        private RedditPost PostOne
        {
            get
            {
                return new RedditPost
                {
                    Id = "ThisIsPostOne",
                    Title = "Anything"
                };
            }
        }

        private RedditPost PostOneClone
        {
            get
            {
                return new RedditPost
                {
                    Id = "ThisIsPostOne",
                    Title = "AnythingElse"
                };
            }
        }

        private RedditPost PostTwo
        {
            get
            {
                return new RedditPost
                {
                    Id = "ThisIsPostTwo",
                    Title = "Anything"
                };
            }
        }

        [TestMethod]
        public void Operators()
        {
            Assert.IsTrue(PostOne == PostOneClone);
            Assert.IsTrue(PostOne != PostTwo);
            Assert.IsFalse(PostOne.Equals(null));
            Assert.IsFalse(PostOne.Equals(new List<int>()));
            Assert.IsTrue(PostOne.Equals(PostOneClone));
            Assert.IsTrue(PostOne.GetHashCode() == PostOneClone.GetHashCode());
            Assert.IsTrue(PostOne.GetHashCode() != PostTwo.GetHashCode());
        }
    }
}
