using E_Government.Core.Domain.RepositoryContracts.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Government.Infrastructure.Common
{
    public abstract  class DbInitializer : IDbInitializer
    {
        private readonly DbContext _dbContext;

        public DbInitializer(DbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task InitializerAsync()
        {
            var pendingMigration = _dbContext.Database.GetPendingMigrations();

            if(pendingMigration.Any()) 
                await _dbContext.Database.MigrateAsync();
        }

        public abstract Task SeedAsync();
       
    }
}
