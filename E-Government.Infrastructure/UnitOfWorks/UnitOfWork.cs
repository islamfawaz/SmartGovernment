using E_Government.Core.Domain.RepositoryContracts.Persistence;
using E_Government.Infrastructure.EGovernment_Unified;
using E_Government.Infrastructure.Generic_Repository;
using System.Collections.Concurrent;

namespace E_Government.Infrastructure.UnitOfWork
{
   public class UnitOfWork : IUnitOfWork
    {
        private readonly UnifiedDbContext _dbContext;
        private readonly ConcurrentDictionary<string, object> _repositories;

        public UnitOfWork(UnifiedDbContext dbContext)
        {
            _dbContext = dbContext;
            _repositories = new ConcurrentDictionary<string, object>();

        }
        public async Task<int> CompleteAsync()
        {
          return  await _dbContext.SaveChangesAsync();
        }

        public async ValueTask DisposeAsync()
        {
          await  _dbContext.DisposeAsync();
        }

        public IGenericRepository<TEntity> GetRepository<TEntity>()
            where TEntity : class
        {
            return (IGenericRepository<TEntity>)_repositories.GetOrAdd(typeof(TEntity).Name, new GenericRepository<TEntity>(_dbContext));

        }
    }
}
