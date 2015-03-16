using AE.EF.Abstract;
using AE.Snipplets.Dal;
using AE.Snipplets.Entity;
using AE.WebUI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace AE.WebUI.Controllers.View
{
    public class Dummy
    {
        public string SomeField { get; set; }
    }

    [Authorize]
    public class CSharpController : Controller
    {
        #region fields and constructor
        internal IBasicRepository _repo;
        public CSharpController(IBasicRepository repo)
        {
            _repo = repo;
        }
        #endregion

        #region views
        [AllowAnonymous]
        [HttpGet]
        public ActionResult Index()
        {
            var model = from s in _repo.Query<Snipplet>()
                        where s.Tags.Any(t => t.TagId == CsTagId)
                        select new CSharpSnippletListItem { Id = s.SnippletId, Headline = s.Headline };
            return View(model);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View(new Snipplet());
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Snipplet snipplet)
        {
            if (snipplet.Headline != null && snipplet.Content != null)
            {
                snipplet.Tags.Add(CsTag);
                _repo.Insert(snipplet);
                await _repo.CommitAsync();
                return RedirectToAction("Index");
            }
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id != null)
            {
                try
                {
                    Snipplet s = _repo.Find<Snipplet>((int)id);
                    return View(s);
                }
                catch (InvalidOperationException)
                {
                    // noop
                }
            }
            return new HttpStatusCodeResult(HttpStatusCode.NotFound);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Snipplet snipplet)
        {
            if (ModelState.IsValid)
            {
                _repo.Update(snipplet);
                await _repo.CommitAsync();
                return RedirectToAction("Index");
            }
            ModelState.AddModelError("", "Data not valid.");
            return View(snipplet);
        }

        [HttpGet]
        public ActionResult Delete(int? id)
        {
            if (id != null)
            {
                try
                {
                    Snipplet s = _repo.Find<Snipplet>((int)id);
                    return View(s);
                }
                catch (InvalidOperationException)
                {
                    // noop
                }
            }
            return new HttpStatusCodeResult(HttpStatusCode.NotFound);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                Snipplet s = _repo.Find<Snipplet>(id);
                _repo.Delete(s);
                await _repo.CommitAsync();
                return RedirectToAction("Index");
            }
            catch (InvalidOperationException)
            {
                // noop
            }
            return new HttpStatusCodeResult(HttpStatusCode.NotFound);
        }
        #endregion

        #region helpers
        internal Tag _csTag;
        internal Tag CsTag
        {
            get
            {
                if (_csTag == null)
                {
                    try
                    {
                        _csTag = _repo.Query<Tag>().First(t => t.Name == "C#");
                    }
                    catch (InvalidOperationException)
                    {
                        _repo.Insert(new Tag { Name = "C#" });
                        _repo.Commit();
                        _csTag = _repo.Query<Tag>().First(t => t.Name == "C#");
                    }
                }
                return _csTag;
            }
        }
        internal int CsTagId
        {
            get
            {
                return CsTag.TagId;
            }
        }
        #endregion
    }
}