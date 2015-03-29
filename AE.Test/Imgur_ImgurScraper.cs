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
    }
}
