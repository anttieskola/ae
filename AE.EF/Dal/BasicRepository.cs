using AE.EF.Abstract;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace AE.EF.Dal
{
    /// <summary>
    /// This provides basic repository functionality implementation for
    /// controllers.
    /// 
    /// Extent this with specific repository you want to create and provide
    /// database context to base in constructor. Then inject or create it in
    /// controller and use.
    /// </summary>
    public class BasicRepository : IBasicRepository
    {
        internal readonly DbContext _db;

        public BasicRepository(DbContext db)
        {
            _db = db;
        }

        #region Basic entity functions
        public virtual void Insert<SetType>(SetType t) where SetType : class
        {
            _db.Entry(t).State = EntityState.Added;
        }

        public virtual void Update<SetType>(SetType t) where SetType : class
        {
            _db.Entry(t).State = EntityState.Modified;
        }

        public virtual void Delete<SetType>(SetType t) where SetType : class
        {
            _db.Entry(t).State = EntityState.Deleted;
        }

        #endregion

        #region Commit / Save after basic functions
        public virtual void Commit()
        {
            _db.SaveChanges();
        }

        public virtual async Task CommitAsync()
        {
            await _db.SaveChangesAsync();
        }
        #endregion

        #region Query
        public virtual SetType Find<SetType>(int id) where SetType : class
        {
            return _db.Set<SetType>().Find(id);
        }

        public virtual IQueryable<SetType> Query<SetType>() where SetType : class
        {
            return _db.Set<SetType>();
        }
        #endregion

        #region dispose pattern
        public void Dispose()
        {
            _db.Dispose();
        }
        #endregion
    }
}
