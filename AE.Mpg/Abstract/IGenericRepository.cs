using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AE.Mpg.Abstract
{
    public interface IGenericRepository<Temp> where Temp : class
    {
        /// <summary>
        /// query get
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="orderBy"></param>
        /// <param name="includeProperties"></param>
        /// <returns></returns>
        IEnumerable<Temp> Get(
            Expression<Func<Temp, bool>> filter = null,
            Func<IQueryable<Temp>,IOrderedQueryable<Temp>> orderBy = null,
            string includeProperties = "");

        /// <summary>
        /// simple get
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Temp Get(int id);

        /// <summary>
        /// insert new item
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        Temp Insert(Temp v);

        /// <summary>
        /// update item
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        Temp Update(Temp v);

        /// <summary>
        /// delete item
        /// </summary>
        /// <param name="id"></param>
        void Delete(int id);

        /// <summary>
        /// delete item
        /// </summary>
        /// <param name="v"></param>
        void Delete(Temp v);
    }
}
