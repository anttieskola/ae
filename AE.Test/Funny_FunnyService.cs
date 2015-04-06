using Microsoft.VisualStudio.TestTools.UnitTesting;
using AE.Funny.Service;
using AE.Funny.Entity;
using System.Collections.Generic;

namespace AE.Test
{
    [TestClass]
    public class Funny_FunnyService
    {
        [TestMethod]
        public void fetchPosts()
        {
            FunnyService fs = FunnyService.Instance;
            List<Post> fps = fs.fetchPosts().Result;
            Assert.AreNotEqual(0, fps.Count);
        }

        [TestMethod]
        public void IsPictureLink()
        {
            Assert.IsTrue(FunnyService.isPictureLink("http://kfodksf/something.jpg"));
            Assert.IsTrue(FunnyService.isPictureLink("http://www.plaa.com/something.jpeg"));
            Assert.IsTrue(FunnyService.isPictureLink("http://db.some.fi/plaa/something.png"));
            Assert.IsTrue(FunnyService.isPictureLink("http://something.gif"));
            Assert.IsFalse(FunnyService.isPictureLink("https://www.something.com/4543453"));
            Assert.IsFalse(FunnyService.isPictureLink("http://wwww.reddit/r/doesnotexist/dfasff324324234fasdf"));
            Assert.IsFalse(FunnyService.isPictureLink("http://imgur.com/gallery/2GfQtZ0"));
            Assert.IsFalse(FunnyService.isPictureLink("https://www.google.com/maps/@38.921194,-77.041887,3a,75y,82.02h,79.5t/data=!3m5!1e1!3m3!1sIdCmgjET_rtLHHlpFFFryw!2e0!3e2"));
        }

        [TestMethod]
        public void IsPictureLinkNull()
        {
            Assert.IsFalse(FunnyService.isPictureLink(null));
        }

        [TestMethod]
        public void CreatePictureLink()
        {
            string ok = FunnyService.createPictureLink("http://anttieskola.azurewebsites.net/Images/mcsd.png").Result;
            Assert.AreNotEqual(null, ok);

            string notOk = FunnyService.createPictureLink("http://anttieskola.azurewebsites.net/Images/mcsd.fpokepwof").Result;
            Assert.AreEqual(null, notOk);

            string imgurOk = FunnyService.createPictureLink("http://imgur.com/UD5PpJs").Result;
            Assert.AreNotEqual(null, imgurOk);
        }
    }
}