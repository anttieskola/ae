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
    public class VehicleRepository : IGenericRepository<Vehicle>
    {
        private GenericRepository<Vehicle> _repo;

        public VehicleRepository()
        {
            _repo = new GenericRepository<Vehicle>(new MpgContext());
        }

        public IEnumerable<Vehicle> Get(
            Expression<Func<Vehicle, bool>> filter = null,
            Func<IQueryable<Vehicle>, IOrderedQueryable<Vehicle>> orderBy = null,
            string includeProperties = "")
        {
            return _repo.Get(filter, orderBy, includeProperties);
        }

        public Vehicle Get(int id)
        {
            return _repo.Get(id);
        }

        public Vehicle Insert(Vehicle v)
        {
            return _repo.Insert(v);
        }

        public Vehicle Update(Vehicle v)
        {
            return _repo.Update(v);
        }

        public void Delete(int id)
        {
            _repo.Delete(id);
        }

        public void Delete(Vehicle v)
        {
            _repo.Delete(v);
        }
    }
}
