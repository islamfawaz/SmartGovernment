using E_Government.Core.Domain.Entities;
using E_Government.Core.Domain.RepositoryContracts.Persistence;
using E_Government.Infrastructure.EGovernment_Unified;
using Microsoft.EntityFrameworkCore;

namespace E_Government.Infrastructure.Generic_Repository
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity>
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

        public async Task<TEntity?> GetAsync(int id)
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

        // New methods for Specification pattern
        public async Task<TEntity?> GetFirstOrDefaultWithSpecAsync(ISpecification<TEntity> spec)
        {
            return await ApplySpecification(spec).FirstOrDefaultAsync();
        }

        public async Task<IReadOnlyList<TEntity>> GetAllWithSpecAsync(ISpecification<TEntity> spec)
        {
            return await ApplySpecification(spec).ToListAsync();
        }

        public async Task<int> CountAsync(ISpecification<TEntity> spec)
        {
            return await ApplySpecification(spec, true).CountAsync();
        }

        private IQueryable<TEntity> ApplySpecification(ISpecification<TEntity> spec, bool forCount = false)
        {
            var query = _dbContext.Set<TEntity>().AsQueryable();

            // Apply criteria (WHERE clause)
            if (spec.Criteria != null)
            {
                query = query.Where(spec.Criteria);
            }

            // Apply includes (eager loading)
            query = spec.Includes.Aggregate(query,
                (current, include) => current.Include(include));

            // Skip ordering if we're just counting
            if (!forCount)
            {
                // Apply ordering
                if (spec.OrderBy != null)
                {
                    query = query.OrderBy(spec.OrderBy);
                }
                else if (spec.OrderByDescending != null)
                {
                    query = query.OrderByDescending(spec.OrderByDescending);
                }
            }

            return query;
        }
    }
}