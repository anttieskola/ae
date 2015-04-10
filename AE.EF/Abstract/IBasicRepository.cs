using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AE.EF.Abstract
{
    /// <summary>
    /// Basic repository interface definition for injection
    /// </summary>
    public interface IBasicRepository: IDisposable
    {
        void Insert<EntityType>(EntityType e) where EntityType : class;
        void Update<EntityType>(EntityType e) where EntityType : class;
        void Delete<EntityType>(EntityType e) where EntityType : class;
        void Delete<EntityType>(List<EntityType> le) where EntityType : class;
        void Delete<EntityType>(int id) where EntityType : class;
        void Commit();
        Task CommitAsync();
        EntityType Find<EntityType>(int id) where EntityType : class;
        IQueryable<EntityType> Query<EntityType>() where EntityType : class;
    }
}
