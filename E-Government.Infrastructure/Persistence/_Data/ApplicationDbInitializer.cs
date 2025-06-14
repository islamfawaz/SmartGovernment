using E_Government.Domain.Entities;
using E_Government.Domain.RepositoryContracts.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace E_Government.Infrastructure.Persistence._Data
{
    public class ApplicationDbInitializer : IApplicationDbInitializer
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public ApplicationDbInitializer(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public async Task InitializerAsync()
        {
            var pendingMigration = _dbContext.Database.GetPendingMigrations();

            if (pendingMigration.Any())
                await _dbContext.Database.MigrateAsync();
        }

        public async Task SeedAsync()
        {
            bool roleExists=await _roleManager.RoleExistsAsync("Admin");
            if (!roleExists)
            {
                var role = new IdentityRole("Admin");
                await _roleManager.CreateAsync(role);
            }
            var adminEmail = "islamfawaz@gmail.com";
            var adminUserName = "islam.fawaz.admin";

            var user = await _userManager.FindByEmailAsync(adminEmail);
            if (user == null)
            {
                user = new ApplicationUser
                {
                    NID="12345678901234", 
                    UserName = adminUserName,
                    DisplayName="Islam Fawaz",
                    Email = adminEmail,
                    PhoneNumber = "0112334455",
                    EmailConfirmed = true,
                    Address = "Ismailila",
                };
                user.NID = user.Id;

                var result = await _userManager.CreateAsync(user, "P@ssw0rd");

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Admin");
                }
            }
            else
            {
                // User exists, ensure they are in Admin role
                if (!await _userManager.IsInRoleAsync(user, "Admin"))
                {
                    await _userManager.AddToRoleAsync(user, "Admin");
                    Console.WriteLine($"Existing user {adminEmail} added to Admin role.");
                }
                else
                {
                    Console.WriteLine($"User {adminEmail} already exists and is in Admin role.");
                }
            }

            ///public  async Task SeedAsync()
            //{
            //    var jsonOptions = new JsonSerializerOptions
            //    {
            //        PropertyNameCaseInsensitive = true,
            //        Converters = { new JsonStringEnumConverter() }
            //    };

            //    // Seed Customers
            //    if (!_dbContext.ApplicationUsers.Any())
            //    {
            //        var CitizensData = await File.ReadAllTextAsync("../E-Government.Infrastructure\\_Data\\Seeds\\Users.json");
            //        var Citizens = JsonSerializer.Deserialize<List<ApplicationUser>>(CitizensData);

            //        if (Citizens?.Count > 0)
            //        {
            //            await _dbContext.ApplicationUsers.AddRangeAsync(Citizens);
            //            await _dbContext.SaveChangesAsync();
            //        }
            //    }


            //    // Seed Meters
            //    if (!_dbContext.Meters.Any())
            //    {
            //        // Clear any tracked entities first
            //        _dbContext.ChangeTracker.Clear();

            //        var meterData = await File.ReadAllTextAsync("../E-Government.Infrastructure\\_Data\\Seeds\\meters.json");
            //        var meters = JsonSerializer.Deserialize<List<Meter>>(meterData, jsonOptions);

            //        if (meters?.Count > 0)
            //        {
            //            // Ensure no duplicates in the JSON data
            //            var distinctMeters = meters
            //                .GroupBy(m => m.Id)
            //                .Select(g => g.First())
            //                .ToList();

            //            await _dbContext.Meters.AddRangeAsync(distinctMeters);
            //            await _dbContext.SaveChangesAsync();
            //        }
            //    }

            //    // Seed Bills
            //    if (!_dbContext.Meters.Any())
            //    {
            //        // Clear any tracked entities first
            //        _dbContext.ChangeTracker.Clear();

            //        var billData = await File.ReadAllTextAsync("../E-Government.Infrastructure\\_Data\\Seeds\\bills.json");
            //        var bills = JsonSerializer.Deserialize<List<Bill>>(billData, jsonOptions);

            //        if (bills?.Count > 0)
            //        {


            //            await _dbContext.Bills.AddRangeAsync(bills);
            //            await _dbContext.SaveChangesAsync();
            //        }
            //    }



            //    // In your seed method for MeterReadings
            //    if (!_dbContext.MeterReadings.Any())
            //    {
            //        // Get first two meters that exist
            //        var existingMeters = await _dbContext.Meters
            //            .OrderBy(m => m.Id)
            //            .Take(2)
            //            .ToListAsync();

            //        if (existingMeters.Count >= 2)
            //        {
            //            var readings = new List<MeterReading>
            //{
            //    new MeterReading
            //    {
            //        MeterId = existingMeters[0].Id,
            //        ReadingDate = new DateTime(2023, 1, 1),
            //        Value = 1250,
            //        IsEstimated = false
            //    },
            //    new MeterReading
            //    {
            //        MeterId = existingMeters[0].Id,
            //        ReadingDate = new DateTime(2023, 2, 1),
            //        Value = 1450,
            //        IsEstimated = false
            //    },
            //    new MeterReading
            //    {
            //        MeterId = existingMeters[1].Id,
            //        ReadingDate = new DateTime(2023, 1, 1),
            //        Value = 50,
            //        IsEstimated = false
            //    }
            //};

            //            await _dbContext.MeterReadings.AddRangeAsync(readings);
            //            await _dbContext.SaveChangesAsync();
            //        }
            //    }
            //}
        }
    }
}
