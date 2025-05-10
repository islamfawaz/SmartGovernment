using E_Government.Core.Domain.Entities;
using E_Government.Core.Domain.RepositoryContracts.Persistence;
using E_Government.Infrastructure.EGovernment_Unified;
using Microsoft.EntityFrameworkCore;

namespace E_Government.Infrastructure.Generic_Repository
{
    public class GenericRepository<TEntity, TKey> : IGenericRepository<TEntity, TKey>
        where TEntity : class
    {
        private readonly UnifiedDbContext _dbContext;

        public GenericRepository(UnifiedDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Existing methods remain the same...
        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _dbContext.Set<TEntity>().AsNoTracking().ToListAsync();
        }

        public async Task<TEntity?> GetAsync(TKey id)
        {
            return await _dbContext.Set<TEntity>().FindAsync(id);
        }
        public async Task<ApplicationUser> GetUserByNID(string NID)
        {
            return await _dbContext.Set<ApplicationUser>().Where(C=>C.NID == NID).FirstOrDefaultAsync();
        }

        public async Task AddAsync(TEntity entity)
        {
            await _dbContext.AddAsync(entity);
        }


        public void Delete(TEntity entity)
        {
            _dbContext.Remove(entity);
        }

        public void Update(TEntity entity)
        {
            _dbContext.Update(entity);
        }

        // New include-based methods
        public async Task<TEntity?> GetByIdWithIncludeAsync(TKey id, Func<IQueryable<TEntity>, IQueryable<TEntity>>? include = null)
        {
            IQueryable<TEntity> query = _dbContext.Set<TEntity>();
            if (include != null)
                query = include(query);
            return await query.FirstOrDefaultAsync(e => EF.Property<TKey>(e, "Id")!.Equals(id));
        }

        public async Task<IEnumerable<TEntity>> GetAllWithIncludeAsync(Func<IQueryable<TEntity>, IQueryable<TEntity>>? include = null)
        {
            IQueryable<TEntity> query = _dbContext.Set<TEntity>();
            if (include != null)
                query = include(query);
            return await query.ToListAsync();
        }
    }
}