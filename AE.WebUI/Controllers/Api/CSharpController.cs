using AE.EF.Abstract;
using AE.Snipplets.Dal;
using AE.Snipplets.Entity;
using AE.WebUI.ViewModels;
using System;
using System.Web.Http;

namespace AE.WebUI.Controllers.Api
{
    public class CSharpController : ApiController
    {
        #region fields and constructor
        internal IBasicRepository _repo;
        public CSharpController(IBasicRepository repo)
        {
            _repo = repo;
        }
        #endregion

        [HttpGet]
        public CSharpSnippletContentItem Snipplet(int? id)
        {
            if (id == null)
            {
                return null;
            }
            try
            {
                Snipplet snipplet = _repo.Find<Snipplet>((int)id);
                return new CSharpSnippletContentItem { Headline = snipplet.Headline, Content = snipplet.Content };
            }
            catch (InvalidOperationException)
            {
                // most likely invalid id
            }
            return null;
        }
    }
}
