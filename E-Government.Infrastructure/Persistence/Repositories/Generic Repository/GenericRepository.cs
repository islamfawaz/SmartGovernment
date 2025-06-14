using E_Government.Domain.Entities;
using E_Government.Domain.RepositoryContracts.Persistence;
using E_Government.Infrastructure.Persistence._Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace E_Government.Infrastructure.Persistence.Generic_Repository
{
    public class GenericRepository<TEntity, TKey> : IGenericRepository<TEntity, TKey>
           where TEntity : class
    {
        private readonly ApplicationDbContext _dbContext;

        public GenericRepository(ApplicationDbContext dbContext)
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
            return await _dbContext.Set<ApplicationUser>().Where(C => C.NID == NID).FirstOrDefaultAsync();
        }

        public async Task AddAsync(TEntity entity)
        {
            await _dbContext.AddAsync(entity);
        }

        public async Task<TEntity?> GetRequest(Guid id)
        {
            return await _dbContext.Set<TEntity>().FindAsync(id);
        }

        public async Task<int> CountAsync()
        {
            return await _dbContext.Set<TEntity>().CountAsync();
        }
        public async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _dbContext.Set<TEntity>().CountAsync(predicate);
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

        public async Task<Dictionary<string, int>> GetStatusCountsAsync(Expression<Func<TEntity, bool>> baseFilter = null)
        {
            var query = _dbContext.Set<TEntity>().AsQueryable();
            if (baseFilter != null)
            {
                query = query.Where(baseFilter);
            }

            // Modified to include "new" as a pending status, case-insensitively.
            return new Dictionary<string, int>
            {
                ["Total"] = await query.CountAsync(),
                ["Pending"] = await query.CountAsync(r => EF.Property<string>(r, "Status") != null &&
                                                        (EF.Property<string>(r, "Status").ToLower() == "pending" || EF.Property<string>(r, "Status").ToLower() == "new")),
                ["Approved"] = await query.CountAsync(r => EF.Property<string>(r, "Status") != null && EF.Property<string>(r, "Status").ToLower() == "approved"),
                ["Rejected"] = await query.CountAsync(r => EF.Property<string>(r, "Status") != null && EF.Property<string>(r, "Status").ToLower() == "rejected")
            };
        }



        public async Task<PagedResult<TEntity>> GetPagedListAsync(int pageNumber, int pageSize, Expression<Func<TEntity, bool>> predicate = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, Func<IQueryable<TEntity>, IQueryable<TEntity>> include = null)
        {
            IQueryable<TEntity> query = _dbContext.Set<TEntity>();

            if (include != null)
            {
                query = include(query);
            }

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            var totalCount = await query.CountAsync();

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();

            return new PagedResult<TEntity>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

    }
}