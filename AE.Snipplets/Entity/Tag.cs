using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AE.Snipplets.Entity
{
    public class Tag
    {
        public Tag()
        {
            Snipplets = new HashSet<Snipplet>();
        }

        public int TagId { get; set; }
        public string Name { get; set; }
        public ICollection<Snipplet> Snipplets { get; set; }
    }
}
