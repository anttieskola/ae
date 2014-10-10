using AE.News.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AE.News.Abstract
{
    public interface INewsRepository
    {
        IEnumerable<NewsArticle> Get(Func<NewsArticle, bool> filter = null);
        NewsArticle Get(int id);
        IEnumerable<string> Tags();
        IEnumerable<NewsArticle> Get(string tag);
    }
}
