using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AE.Imgur.Utils;

namespace AE.Test
{
    [TestClass]
    public class Imgur_ImgurScraper
    {
        [TestMethod]
        public void GetImageUrlFromId()
        {
            Assert.IsNotNull(ImgurScraper.GetImageUrl("CXdOCxi").Result);
        }

        [TestMethod]
        public void GetImageUrlFromUrl()
        {
            Assert.IsNotNull(ImgurScraper.GetImageUrl("http://imgur.com/BSMhjEp").Result);
        }

        [TestMethod]
        public void GetImageUrlFromUrl2()
        {
            Assert.IsNotNull(ImgurScraper.GetImageUrl("http://imgur.com/DwWgZeH").Result);
        }

        [TestMethod]
        public void RemoveParameters()
        {
            string yes = "http://i.imgur.com/UD5PpJs.jpg?1?fb";
            string no = "http://i.imgur.com/UD5PpJs.jpg";
            Assert.AreNotEqual(yes, ImgurScraper.removeParameters(yes));
            Assert.AreEqual(no, ImgurScraper.removeParameters(no));
            Assert.AreEqual("http://i.imgur.com/sgLIGyk.png", ImgurScraper.removeParameters("http://i.imgur.com/sgLIGyk.png?1"));
        }
    }
}
