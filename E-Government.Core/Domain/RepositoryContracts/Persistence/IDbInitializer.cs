using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Government.Core.Domain.RepositoryContracts.Persistence
{
  
        public interface IDbInitializer
        {
            Task InitializerAsync();
           Task SeedAsync();
        }
  
}
