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
        public void fetchPostsMT()
        {
            FunnyService fs = FunnyService.Instance;
            List<Post> fps = fs.fetchPostsMT().Result;
            Assert.AreNotEqual(0, fps.Count);
        }

        [TestMethod]
        public void fetchPostsST()
        {
            FunnyService fs = FunnyService.Instance;
            List<Post> fps = fs.fetchPostsST().Result;
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
        }
    }
}
