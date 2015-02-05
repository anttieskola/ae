using AE.News.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AE.WebUI.ViewModels
{
    public class NewsViewModel
    {
        public IEnumerable<string> Tags { get; set; } // available tags
        public string Tag { get; set; } // current
    }
}