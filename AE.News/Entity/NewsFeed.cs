using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AE.News.Entity
{
    [DebuggerDisplay("{Tag} {Uri}")]
    internal class NewsFeed
    {
        public string Tag { get; set; }
        public Uri Uri { get; set; }
    }
}
