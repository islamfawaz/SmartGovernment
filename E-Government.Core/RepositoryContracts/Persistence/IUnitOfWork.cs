using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Government.Domain.RepositoryContracts.Persistence
{
    public interface IUnitOfWork :IAsyncDisposable

    {
        IGenericRepository<TEntity, TKey> GetRepository<TEntity, TKey>()
            where TEntity : class;
            

     
       

        Task<int> CompleteAsync();

    }
}
