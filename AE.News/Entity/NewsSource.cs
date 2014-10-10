using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AE.News.Entity
{
    [DebuggerDisplay("{Id} {Name} {Uri}")]
    public class NewsSource
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Uri { get; set; }
    }
}
