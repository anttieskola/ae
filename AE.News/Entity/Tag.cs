using System.Collections.Generic;
using System.Diagnostics;

namespace AE.News.Entity
{
    [DebuggerDisplay("{TagId} {Name}")]
    public class Tag
    {
        public Tag()
        {
            Articles = new HashSet<Article>();
        }
        public int TagId { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Article> Articles { get; set; }
        public virtual Feed Feed { get; set; }
    }
}
