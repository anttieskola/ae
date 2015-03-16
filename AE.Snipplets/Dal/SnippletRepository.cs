using AE.EF.Dal;
using AE.Snipplets.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AE.Snipplets.Dal
{
    public class SnippletRepository : BasicRepository
    {
        public SnippletRepository()
            : base(new SnippletContext())
        {
            // noop
        }
    }
}
