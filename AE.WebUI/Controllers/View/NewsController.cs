using AE.EF.Abstract;
using AE.News.Entity;
using AE.WebUI.ViewModels;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace AE.WebUI.Controllers.View
{
    public class NewsController : Controller
    {
        private IBasicRepository _repo;

        public NewsController(IBasicRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult ReadingView(int? id = null)
        {
            if (id != null)
            {
                var article = (from a in _repo.Query<Article>()
                               where a.ArticleId == (int)id
                               select new ArticleModel
                               {
                                   Id = a.ArticleId,
                                   Title = a.Title,
                                   Description = a.Description,
                                   Content = a.Content,
                                   ImageUrl = a.ImageUrl,
                                   SourceUrl = a.SourceUrl,
                                   Date = a.Date
                               }).FirstOrDefault();
                if (article != null)
                {
                    return View(article as ArticleModel);
                }
            }
            return new HttpStatusCodeResult(HttpStatusCode.NotFound);
        }
    }
}