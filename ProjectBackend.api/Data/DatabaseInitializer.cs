using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ProjectBackend.api.Models.Domain;

namespace ProjectBackend.api.Data
{
    public class DatabaseInitializer : IDatabaseInitializer
    {
        private readonly ProjectDbContext _dbContext;
        private readonly AdminSeedOptions _adminSeedOptions;

        public DatabaseInitializer(ProjectDbContext dbContext, IOptions<AdminSeedOptions> adminSeedOptions)
        {
            _dbContext = dbContext;
            _adminSeedOptions = adminSeedOptions.Value;
        }

        public async Task InitializeAsync(CancellationToken cancellationToken = default)
        {
            if (_dbContext.Database.IsRelational())
            {
                await _dbContext.Database.MigrateAsync(cancellationToken);
            }
            else
            {
                await _dbContext.Database.EnsureCreatedAsync(cancellationToken);
            }

            if (string.IsNullOrWhiteSpace(_adminSeedOptions.Email) ||
                string.IsNullOrWhiteSpace(_adminSeedOptions.Username) ||
                string.IsNullOrWhiteSpace(_adminSeedOptions.Password))
            {
                return;
            }

            var adminExists = await _dbContext.Users.AnyAsync(user => user.Role == UserRole.Admin, cancellationToken);
            if (adminExists)
            {
                return;
            }

            var adminUser = new UserDomain
            {
                Email = _adminSeedOptions.Email,
                Username = _adminSeedOptions.Username,
                Password = BCrypt.Net.BCrypt.HashPassword(_adminSeedOptions.Password),
                Role = UserRole.Admin
            };

            await _dbContext.Users.AddAsync(adminUser, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
