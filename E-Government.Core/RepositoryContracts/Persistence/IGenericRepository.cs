using E_Government.Domain.Entities;
using System.Linq.Expressions;

namespace E_Government.Domain.RepositoryContracts.Persistence
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
