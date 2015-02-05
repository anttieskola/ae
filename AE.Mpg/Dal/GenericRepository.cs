using AE.Mpg.Abstract;
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
    /// Generic repository. Simple to use for one entity/table, but ofc use is limited.
    /// http://www.asp.net/mvc/tutorials/getting-started-with-ef-5-using-mvc-4/implementing-the-repository-and-unit-of-work-patterns-in-an-asp-net-mvc-application
    /// http://stackoverflow.com/questions/6223075/how-do-i-define-a-generic-class-that-implements-an-interface-and-constrains-the
    /// </summary>
    /// <typeparam name="Temp"></typeparam>
    public class GenericRepository<Temp> : IGenericRepository<Temp> where Temp : class
    {
        internal MpgContext _db;
        internal DbSet<Temp> _set;

        /// <summary>
        /// Default constructor
        /// </summary>
        public GenericRepository()
        {
            _db = new MpgContext();
            _db.Configuration.ProxyCreationEnabled = false; // Solve, proxy objects can't be serialized. Note! now changes to objects won't be updated in db...
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
        public async virtual Task<Temp> Insert(Temp t)
        {
            t = _set.Add(t);
            await _db.SaveChangesAsync();
            return t;
        }

        /// <summary>
        /// update
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public async virtual Task<Temp> Update(Temp t)
        {
            t = _set.Attach(t);
            _db.Entry(t).State = EntityState.Modified;
            await _db.SaveChangesAsync();
            return t;
        }

        /// <summary>
        /// delete
        /// </summary>
        /// <param name="id"></param>
        public async virtual Task Delete(int id)
        {
            Temp t = _set.Find(id);
            _set.Remove(t);
            await _db.SaveChangesAsync();
        }

        /// <summary>
        /// delete
        /// </summary>
        /// <param name="t"></param>
        public async virtual Task Delete(Temp t)
        {
            if (_db.Entry(t).State == EntityState.Detached)
            {
                _set.Attach(t);
            }
            _set.Remove(t);
            await _db.SaveChangesAsync();
        }
    }
}
