using AE.EF.Abstract;
using AE.Funny.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace AE.WebUI.Controllers.View
{
    public class FunnyController : Controller
    {
        private IBasicRepository _repo;

        public FunnyController(IBasicRepository repository)
        {
            _repo = repository;
        }

        public ActionResult Index()
        {
            return View();
        }

        //private void serviceCheck()
        //{
        //    // check when service last ran
        //    FunnyService fs = FunnyService.Instance;
        //    TimeSpan since = DateTime.UtcNow.Subtract(fs.LastSuccess());
        //    if (since.TotalMinutes > 60)
        //    {
        //        // fire and forget
        //        Task.Factory.StartNew(() => fs.Run());
        //    }
        //}
    }
}