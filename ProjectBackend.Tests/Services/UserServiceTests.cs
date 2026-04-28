using ProjectBackend.api.Exceptions;
using ProjectBackend.api.Models.Domain;
using ProjectBackend.api.Models.DTO;
using ProjectBackend.api.Services;
using ProjectBackend.Tests.TestInfrastructure;

namespace ProjectBackend.Tests.Services
{
    public class UserServiceTests
    {
        [Fact]
        public async Task CreateAsync_ShouldThrowConflictException_WhenEmailAlreadyExists()
        {
            var repository = new FakeUserRepository();
            repository.Users.Add(new UserDomain
            {
                Id = 1,
                Email = "existing@test.com",
                Username = "existing",
                Password = "hashed",
                Role = UserRole.User
            });

            var service = new UserService(repository, TestMapperFactory.Create());

            var dto = new CreateUserDto
            {
                Email = "existing@test.com",
                Username = "newuser",
                Password = "Password123!",
                Role = UserRole.User
            };

            await Assert.ThrowsAsync<ConflictException>(() => service.CreateAsync(dto));
        }

        [Fact]
        public async Task DeleteAsync_ShouldThrowNotFoundException_WhenUserDoesNotExist()
        {
            var service = new UserService(new FakeUserRepository(), TestMapperFactory.Create());

            await Assert.ThrowsAsync<NotFoundException>(() => service.DeleteAsync(42));
        }
    }
}
