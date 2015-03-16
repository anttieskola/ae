using System.Collections;
using System.Collections.Generic;

namespace AE.Snipplets.Entity
{
    public class Snipplet
    {
        public Snipplet()
        {
            Tags = new HashSet<Tag>();
        }

        public int SnippletId { get; set; }
        public string Headline { get; set; }
        public string Content { get; set; }
        public virtual ICollection<Tag> Tags { get; set; }
    }
}
