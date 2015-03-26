using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AE.Imgur.Utils;

namespace AE.Test
{
    [TestClass]
    public class Imgur_ImgurScraper
    {
        [TestMethod]
        public void GetImageUrl()
        {
            ImgurScraper.GetImageUrl("CXdOCxi").Wait();
        }
    }
}
