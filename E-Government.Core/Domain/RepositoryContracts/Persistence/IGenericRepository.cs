using E_Government.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Government.Core.Domain.RepositoryContracts.Persistence
{
   public interface IGenericRepository<TEntity>
        where TEntity : class
    {
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<TEntity ?>GetAsync(int id);
        Task<ApplicationUser> GetUserByNID(string NID);
        Task AddAsync(TEntity entity);

        void Update(TEntity entity);
        void Delete(TEntity entity);
        Task<IReadOnlyList<TEntity>> GetAllWithSpecAsync(ISpecification<TEntity> spec);
        Task<TEntity?> GetFirstOrDefaultWithSpecAsync(ISpecification<TEntity> spec);

        Task<int> CountAsync(ISpecification<TEntity> spec);

    }
}
