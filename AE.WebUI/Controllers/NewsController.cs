﻿using AE.News;
using AE.News.Abstract;
using AE.News.Entity;
using AE.WebUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace AE.WebUI.Controllers
{
    public class NewsController : Controller
    {
        private const string defaultTag = "In English";

        [HttpGet]
        public async Task<ViewResult> Index()
        {
            NewsContext nc = await NewsContext.Instance;
            NewsViewModel vm = new NewsViewModel
            {
                Tags = new List<string>(nc.Tags()),
                Tag = nc.Tags().First()
            };
            return View(vm);
        }

        [HttpGet]
        public async Task<PartialViewResult> News(string tag = null)
        {
            NewsContext nc = await NewsContext.Instance;
            if (tag == null)
            {
                // get default or first
                if (nc.Tags().Any(t => t.Equals(defaultTag)))
                {
                    return PartialView("_News", nc.Get(a => a.Tag.Contains(defaultTag)));
                }
                else
                {
                    return PartialView("_News", nc.Get(a => a.Tag.Contains(nc.Tags().First())));
                }
            }
            // all articles
            if (tag.Equals("*"))
            {
                return PartialView("_News", nc.Get());
            }
            // just given tag
            return PartialView("_News", nc.Get(a => a.Tag.Contains(tag)));
        }

        [HttpGet]
        public async Task<ViewResult> ReadingView(int articleId)
        {
            NewsContext nc = await NewsContext.Instance;
            return View(nc.Get((int)articleId));
        }
    }
}