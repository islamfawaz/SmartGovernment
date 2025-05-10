// E-Government.Infrastructure/Persistence/Repository/UnitOfWork.cs
using E_Government.Core.Domain.RepositoryContracts.Persistence;
using E_Government.Infrastructure.EGovernment_Unified; // Assuming this is the correct namespace for UnifiedDbContext
using E_Government.Infrastructure.Generic_Repository; // Added for GenericRepository
using System.Collections;
using System.Threading.Tasks;

namespace E_Government.Infrastructure.Persistence.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly UnifiedDbContext _context;
        private Hashtable? _repositories; // Cache for repository instances

        public UnitOfWork(UnifiedDbContext context)
        {
            _context = context;
        }

        public IGenericRepository<TEntity, TKey> GetRepository<TEntity, TKey>() where TEntity : class
        {
            // Initialize cache if null
            if (_repositories == null) _repositories = new Hashtable();

            var type = typeof(TEntity).Name + typeof(TKey).Name;

            // Check if repository already exists in cache
            if (!_repositories.ContainsKey(type))
            {
                // If not, create a new instance of the GenericRepository<TEntity, TKey>
                var repositoryType = typeof(GenericRepository<,>);
                var repositoryInstance = Activator.CreateInstance(repositoryType.MakeGenericType(typeof(TEntity), typeof(TKey)), _context);

                // Add the new repository instance to the cache
                if (repositoryInstance != null)
                {
                    _repositories.Add(type, repositoryInstance);
                }
                else
                {
                    // Handle potential error during Activator.CreateInstance
                    throw new InvalidOperationException($"Could not create repository instance for {type}");
                }
            }

            // Return the repository instance from the cache
            return (IGenericRepository<TEntity, TKey>)_repositories[type]!;
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }

        public async ValueTask DisposeAsync()
        {
            await _context.DisposeAsync();
            GC.SuppressFinalize(this);
        }
    }
}

