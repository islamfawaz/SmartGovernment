using E_Government.Core.Domain.Entities;
using E_Government.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace E_Government.Core.Domain.RepositoryContracts.Persistence
{
   public interface IGenericRepository<TEntity, TKey>
        where TEntity : class
    {
        Task<Dictionary<string, int>> GetStatusCountsAsync(Expression<Func<TEntity, bool>> baseFilter = null);
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<TEntity?> GetAsync(TKey id);
        Task<ApplicationUser> GetUserByNID(string NID);
        Task AddAsync(TEntity entity);

        void Update(TEntity entity);
        void Delete(TEntity entity);
        Task<TEntity?> GetByIdWithIncludeAsync(TKey id, Func<IQueryable<TEntity>, IQueryable<TEntity>>? include = null);
        Task<IEnumerable<TEntity>> GetAllWithIncludeAsync(Func<IQueryable<TEntity>, IQueryable<TEntity>>? include = null);
        Task<TEntity?> GetRequest(Guid id);

        Task<int> CountAsync();

        Task<PagedResult<TEntity>> GetPagedListAsync(
       int pageNumber,
       int pageSize,
       Expression<Func<TEntity, bool>> predicate = null,
      Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
      Func<IQueryable<TEntity>, IQueryable<TEntity>> include = null);


        Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate); // Add this line


    }
}
