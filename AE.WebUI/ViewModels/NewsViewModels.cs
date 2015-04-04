using AE.News.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AE.WebUI.ViewModels
{
    public class TagModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class ArticleModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }
        public string ImageUrl { get; set; }
        public string SourceUrl { get; set; }
        public DateTime Date { get; set; }
    }
}