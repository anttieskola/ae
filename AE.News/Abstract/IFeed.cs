using AE.News.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AE.News.Abstract
{
    internal interface IFeed
    {
        Task<IEnumerable<NewsArticle>> Fetch();
    }
}
