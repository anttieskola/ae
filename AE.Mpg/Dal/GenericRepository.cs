using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AE.Mpg.Dal
{
    /// <summary>
    /// http://www.asp.net/mvc/tutorials/getting-started-with-ef-5-using-mvc-4/implementing-the-repository-and-unit-of-work-patterns-in-an-asp-net-mvc-application
    /// </summary>
    /// <typeparam name="Temp"></typeparam>
    internal class GenericRepository<Temp> where Temp : class
    {
        internal MpgContext _db;
        internal DbSet<Temp> _set;

        public GenericRepository(MpgContext db)
        {
            _db = db;
            _set = _db.Set<Temp>();
        }


        /// <summary>
        /// queriable get
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="orderBy"></param>
        /// <param name="includeProperties"></param>
        /// <returns></returns>
        public virtual IEnumerable<Temp> Get(
            Expression<Func<Temp, bool>> filter = null,
            Func<IQueryable<Temp>,IOrderedQueryable<Temp>> orderBy = null,
            string includeProperties = "")
        {
            IQueryable<Temp> q = _set;

            // lambda expression
            if (filter != null)
            {
                q = q.Where(filter);
            }

            // ?
            foreach (var ip in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                q = q.Include(ip);
            }

            // order or no?
            if (orderBy != null)
            {
                return orderBy(q).ToList();
            }
            return q.ToList();
        }

        /// <summary>
        /// simple get
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual Temp Get(int id)
        {
            return _set.Find(id);
        }

        /// <summary>
        /// insert
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public virtual Temp Insert(Temp t)
        {
            return _set.Add(t);
        }

        /// <summary>
        /// update
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public virtual Temp Update(Temp t)
        {
            t = _set.Attach(t);
            _db.Entry(t).State = EntityState.Modified;
            return t;
        }

        /// <summary>
        /// delete
        /// </summary>
        /// <param name="id"></param>
        public virtual void Delete(int id)
        {
            Temp t = _set.Find(id);
            _set.Remove(t);
        }

        /// <summary>
        /// delete
        /// </summary>
        /// <param name="t"></param>
        public virtual void Delete(Temp t)
        {
            if (_db.Entry(t).State == EntityState.Detached)
            {
                _set.Attach(t);
            }
            _set.Remove(t);
        }
    }
}
