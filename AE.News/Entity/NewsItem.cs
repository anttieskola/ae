using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace AE.News.Entity
{
    [DebuggerDisplay("{Title} {Description} {Content}")]
    public class NewsItem
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }
        public Uri Image { get; set; }
        public Uri Origin { get; set; }
    }
}