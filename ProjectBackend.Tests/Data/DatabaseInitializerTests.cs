using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ProjectBackend.api.Data;
using ProjectBackend.api.Models.Domain;

namespace ProjectBackend.Tests.Data
{
    public class DatabaseInitializerTests
    {
        [Fact]
        public async Task InitializeAsync_ShouldSeedAdmin_WhenAdminDoesNotExist()
        {
            var options = new DbContextOptionsBuilder<ProjectDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            await using var context = new ProjectDbContext(options);
            var seedOptions = Options.Create(new AdminSeedOptions
            {
                Email = "admin@test.local",
                Username = "admin",
                Password = "Admin123!"
            });

            var initializer = new DatabaseInitializer(context, seedOptions);

            await initializer.InitializeAsync();

            var admin = await context.Users.FirstOrDefaultAsync(user => user.Role == UserRole.Admin);
            Assert.NotNull(admin);
            Assert.Equal("admin", admin.Username);
        }
    }
}
