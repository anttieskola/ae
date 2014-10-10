using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AE.News.Entity
{
    public class NewsArticle
    {
        public int Id { get; set; }
        public List<string> Tag { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }
        public Uri Image { get; set; }
        public Uri Source { get; set; }
        public DateTime Date { get; set; }
        public int Hash { get; set; }

        public NewsArticle()
        {
            Tag = new List<string>();
        }
    }
}
