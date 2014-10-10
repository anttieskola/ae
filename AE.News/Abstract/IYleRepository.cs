using AE.News.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AE.News.Abstract
{
    public interface IYleRepository
    {
        IEnumerable<NewsSource> Sources { get; }
        Task<IEnumerable<NewsItem>> News(NewsSource source);
    }
}
