using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AE.Funny;
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
            FunnyService fs = new FunnyService();
            List<FunnyPost> fps = fs.fetchPosts().Result;
            Assert.AreNotEqual(0, fps.Count);
        }

        [TestMethod]
        public void fetchPostsMT()
        {
            FunnyService fs = new FunnyService();
            List<FunnyPost> fps = fs.fetchPostsMT().Result;
            Assert.AreNotEqual(0, fps.Count);
        }

        [TestMethod]
        public void fetchPostsST()
        {
            FunnyService fs = new FunnyService();
            List<FunnyPost> fps = fs.fetchPostsST().Result;
            Assert.AreNotEqual(0, fps.Count);
        }

        [TestMethod]
        public void IsPictureLink()
        {
            Assert.IsTrue(FunnyService.IsPictureLink("http://kfodksf/something.jpg"));
            Assert.IsTrue(FunnyService.IsPictureLink("http://www.plaa.com/something.jpeg"));
            Assert.IsTrue(FunnyService.IsPictureLink("http://db.some.fi/plaa/something.png"));
            Assert.IsTrue(FunnyService.IsPictureLink("http://something.gif"));
            Assert.IsFalse(FunnyService.IsPictureLink("https://www.something.com/4543453"));
            Assert.IsFalse(FunnyService.IsPictureLink("http://wwww.reddit/r/doesnotexist/dfasff324324234fasdf"));
            Assert.IsFalse(FunnyService.IsPictureLink("http://imgur.com/gallery/2GfQtZ0"));
        }
    }
}
