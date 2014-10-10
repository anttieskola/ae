using AE.News;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AE.Test
{
    [TestClass]
    public class News_NewsDaemon
    {
        [TestMethod]
        public void Instance()
        {
            NewsDaemon nu = NewsDaemon.Instance;
            Assert.IsNotNull(nu);
        }

        [TestMethod]
        public void StartAndStop()
        {
            Assert.IsTrue(NewsDaemon.Instance.Start());
            Assert.IsTrue(NewsDaemon.Instance.Stop());
        }

        [TestMethod]
        public void WrongUse()
        {
            Assert.IsFalse(NewsDaemon.Instance.Stop());
            Assert.IsTrue(NewsDaemon.Instance.Start());
            Assert.IsFalse(NewsDaemon.Instance.Start());
            Assert.IsTrue(NewsDaemon.Instance.Stop());
            Assert.IsFalse(NewsDaemon.Instance.Stop());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void WrongInterval()
        {
            NewsDaemon.Instance.Start(0);
        }
    }
}
