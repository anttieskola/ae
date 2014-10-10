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
    public class News_NewsContext
    {

        [TestMethod]
        public void Instance()
        {
            NewsContext c = NewsContext.GetInstance().Result;
            Assert.IsNotNull(c);
        }

        [TestMethod]
        public void Tags()
        {
            NewsContext c = NewsContext.GetInstance().Result;
            Assert.IsTrue(c.Tags().Count() > 0);
        }

        [TestMethod]
        public void Get()
        {
            // all
            NewsContext c = NewsContext.GetInstance().Result;
            List<NewsArticle> news = c.Get().ToList();
            Assert.IsTrue(news.Count() > 2);

            // lambda

            // single
            Random r = new Random();
            int index = r.Next(news.Count() - 1);
            NewsArticle article = c.Get(news.ElementAt(index).Id);
            Assert.IsNotNull(article);
        }

        [TestMethod]
        public void plaa()
        {
            NewsContext c = NewsContext.GetInstance().Result;
            foreach (var a in c.Get())
            {
                System.Diagnostics.Debug.WriteLine(a.Title);
            }
        }
    }
}
