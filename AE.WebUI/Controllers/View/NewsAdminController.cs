using AE.EF.Abstract;
using AE.News.Entity;
using AE.WebUI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AE.WebUI.Controllers.View
{
    [Authorize]
    public class NewsAdminController : Controller
    {
        #region fields & constructor
        private IBasicRepository _repo;
        public NewsAdminController(IBasicRepository repo)
        {
            _repo = repo;
        }
        #endregion

        public ActionResult Index()
        {
            NewsAdminViewModel vm = new NewsAdminViewModel();
            vm.Maintenances = (from m in _repo.Query<Maintenance>()
                               /* it has no foreing keys so giving it*/
                               select m).ToList();
            return View(vm);
        }
    }
}