using E_Government.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Government.Core.Domain.RepositoryContracts.Persistence
{
   public interface IGenericRepository<TEntity, TKey>
        where TEntity : class
    {
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<TEntity?> GetAsync(TKey id);
        Task<ApplicationUser> GetUserByNID(string NID);
        Task AddAsync(TEntity entity);

        void Update(TEntity entity);
        void Delete(TEntity entity);
        Task<TEntity?> GetByIdWithIncludeAsync(TKey id, Func<IQueryable<TEntity>, IQueryable<TEntity>>? include = null);
        Task<IEnumerable<TEntity>> GetAllWithIncludeAsync(Func<IQueryable<TEntity>, IQueryable<TEntity>>? include = null);

    }
}
