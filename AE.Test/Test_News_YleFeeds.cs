using AE.News;
using AE.News.Entity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AE.Test
{
    [TestClass]
    public class News_YleFeeds
    {
        [TestMethod]
        public void Fetch()
        {
            YleFeeds yf = new YleFeeds();
            List<NewsArticle> news = new List<NewsArticle>(yf.Fetch().Result);
        }
    }
}
