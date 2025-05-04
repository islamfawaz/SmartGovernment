using E_Government.Core.Domain.Entities;
using E_Government.Core.Domain.RepositoryContracts.Persistence;
using E_Government.Infrastructure.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace E_Government.Infrastructure.EGovernment_Unified
{
    public class ApplicationDbInitializer : DbInitializer, IDbInitializer
    {
        private readonly UnifiedDbContext _dbContext;

        public ApplicationDbInitializer(UnifiedDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public override async Task SeedAsync()
        {
            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter() }
            };

            // Seed Customers
            if (!_dbContext.ApplicationUsers.Any())
            {
                var CitizensData = await File.ReadAllTextAsync("../E-Government.Infrastructure\\_Data\\Seeds\\Users.json");
                var Citizens = JsonSerializer.Deserialize<List<ApplicationUser>>(CitizensData);

                if (Citizens?.Count > 0)
                {
                    await _dbContext.ApplicationUsers.AddRangeAsync(Citizens);
                    await _dbContext.SaveChangesAsync();
                }
            }


            // Seed Meters
            if (!_dbContext.Meters.Any())
            {
                // Clear any tracked entities first
                _dbContext.ChangeTracker.Clear();

                var meterData = await File.ReadAllTextAsync("../E-Government.Infrastructure\\_Data\\Seeds\\meters.json");
                var meters = JsonSerializer.Deserialize<List<Meter>>(meterData, jsonOptions);

                if (meters?.Count > 0)
                {
                    // Ensure no duplicates in the JSON data
                    var distinctMeters = meters
                        .GroupBy(m => m.Id)
                        .Select(g => g.First())
                        .ToList();

                    await _dbContext.Meters.AddRangeAsync(distinctMeters);
                    await _dbContext.SaveChangesAsync();
                }
            }

            // Seed Bills
            if (!_dbContext.Meters.Any())
            {
                // Clear any tracked entities first
                _dbContext.ChangeTracker.Clear();

                var billData = await File.ReadAllTextAsync("../E-Government.Infrastructure\\_Data\\Seeds\\bills.json");
                var bills = JsonSerializer.Deserialize<List<Bill>>(billData, jsonOptions);

                if (bills?.Count > 0)
                {
                  

                    await _dbContext.Bills.AddRangeAsync(bills);
                    await _dbContext.SaveChangesAsync();
                }
            }



            // In your seed method for MeterReadings
            if (!_dbContext.MeterReadings.Any())
            {
                // Get first two meters that exist
                var existingMeters = await _dbContext.Meters
                    .OrderBy(m => m.Id)
                    .Take(2)
                    .ToListAsync();

                if (existingMeters.Count >= 2)
                {
                    var readings = new List<MeterReading>
        {
            new MeterReading
            {
                MeterId = existingMeters[0].Id,
                ReadingDate = new DateTime(2023, 1, 1),
                Value = 1250,
                IsEstimated = false
            },
            new MeterReading
            {
                MeterId = existingMeters[0].Id,
                ReadingDate = new DateTime(2023, 2, 1),
                Value = 1450,
                IsEstimated = false
            },
            new MeterReading
            {
                MeterId = existingMeters[1].Id,
                ReadingDate = new DateTime(2023, 1, 1),
                Value = 50,
                IsEstimated = false
            }
        };

                    await _dbContext.MeterReadings.AddRangeAsync(readings);
                    await _dbContext.SaveChangesAsync();
                }
            }
        }
    }
}