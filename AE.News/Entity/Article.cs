using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace AE.News.Entity
{
    [DebuggerDisplay("{ArticleId} {Title}")]
    public class Article
    {
        public Article()
        {
            Tags = new HashSet<Tag>();
        }
        public int ArticleId { get; set; }
        public ICollection<Tag> Tags { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }
        public string ImageUrl { get; set; }
        public string SourceUrl { get; set; }
        public DateTime Date { get; set; }
    }
}
