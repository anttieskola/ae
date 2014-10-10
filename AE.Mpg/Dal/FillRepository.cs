using AE.Mpg.Abstract;
using AE.Mpg.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AE.Mpg.Dal
{
    public class FillRepository : IGenericRepository<Fill>
    {
        private GenericRepository<Fill> _repo;

        public FillRepository()
        {
            _repo = new GenericRepository<Fill>(new MpgContext());
        }

        public IEnumerable<Fill> Get(
            Expression<Func<Fill, bool>> filter = null,
            Func<IQueryable<Fill>, IOrderedQueryable<Fill>> orderBy = null,
            string includeProperties = "")
        {
            return _repo.Get(filter, orderBy, includeProperties);
        }

        public Fill Get(int id)
        {
            return _repo.Get(id);
        }

        public Fill Insert(Fill f)
        {
            return _repo.Insert(f);
        }

        public Fill Update(Fill f)
        {
            return _repo.Update(f);
        }

        public void Delete(int id)
        {
            _repo.Delete(id);
        }

        public void Delete(Fill f)
        {
            _repo.Delete(f);
        }
    }
}
