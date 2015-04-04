using AE.EF.Dal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AE.News.Dal
{
    public class NewsRepository : BasicRepository
    {
        public NewsRepository()
            : base(new NewsContext())
        {
        }
    }
}
