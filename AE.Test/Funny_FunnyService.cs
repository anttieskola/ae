using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AE.Funny;

namespace AE.Test
{
    [TestClass]
    public class Funny_FunnyService
    {
        [TestMethod]
        public void Maintenance()
        {
            FunnyService fs = new FunnyService();
            Assert.AreNotEqual(0, fs.Maintenance());
        }

        [TestMethod]
        public void MaintenanceST()
        {
            FunnyService fs = new FunnyService();
            Assert.AreNotEqual(0, fs.MaintenanceST());
        }

    }
}
