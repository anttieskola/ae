using AE.Imgur.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AE.Test
{
    [TestClass]
    public class Imgur_ImgurClient
    {
        [TestMethod]
        public void Authorize()
        {
            ImgurClient ic = new ImgurClient();
            ic.Authorize("").Wait();
        }
        [TestMethod]
        public void RefreshToken()
        {
            ImgurClient ic = new ImgurClient();
            ic.RefreshToken("","","").Wait();
        }
    }
}
