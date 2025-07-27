// E-Government.Infrastructure/Persistence/Repository/UnitOfWork.cs
using E_Government.Domain.RepositoryContracts.Persistence;
using E_Government.Infrastructure.Persistence._Data;
using E_Government.Infrastructure.Persistence.Generic_Repository;
using E_Government.Infrastructure.Persistence.Repositories;
using System.Collections;

namespace E_Government.Infrastructure.Persistence.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private readonly Lazy<IOtpCodeRepository> _otpCodeRepository;

        private Hashtable? _repositories;


        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            _otpCodeRepository = new Lazy<IOtpCodeRepository>(() => new OtpCodeRepository(_context));

        }

        public IGenericRepository<TEntity, TKey> GetRepository<TEntity, TKey>() where TEntity : class
        {
            if (_repositories == null) _repositories = new Hashtable();

            var type = typeof(TEntity).Name + typeof(TKey).Name;

            if (!_repositories.ContainsKey(type))
            {
                var repositoryType = typeof(GenericRepository<,>);
                var repositoryInstance = Activator.CreateInstance(
                    repositoryType.MakeGenericType(typeof(TEntity), typeof(TKey)),
                    _context
                );

                if (repositoryInstance != null)
                {
                    _repositories.Add(type, repositoryInstance);
                }
                else
                {
                    throw new InvalidOperationException($"Could not create repository instance for {type}");
                }
            }

            return (IGenericRepository<TEntity, TKey>)_repositories[type]!;
        }

        public async Task<int> CompleteAsync() => await _context.SaveChangesAsync();

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

        public IOtpCodeRepository OtpCodeRepository => _otpCodeRepository.Value; 

    }
}

