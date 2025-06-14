using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Government.Domain.RepositoryContracts.Persistence
{
  
        public interface IApplicationDbInitializer
        {
            Task InitializerAsync();
           Task SeedAsync();
        }
  
}
