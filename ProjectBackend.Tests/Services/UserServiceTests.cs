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

            var service = new UserService(repository, TestMapperFactory.Create(), new FakeCurrentUserContext());

            var dto = new CreateUserDto
            {
                Email = "existing@test.com",
                Username = "newuser",
                Password = "Password123!",
                Role = UserRole.User
            };

            await Assert.ThrowsAsync<ConflictException>(() => service.CreateAsync(dto, CancellationToken.None));
        }

        [Fact]
        public async Task DeleteAsync_ShouldThrowNotFoundException_WhenUserDoesNotExist()
        {
            var service = new UserService(new FakeUserRepository(), TestMapperFactory.Create(), new FakeCurrentUserContext());

            await Assert.ThrowsAsync<NotFoundException>(() => service.DeleteAsync(42, CancellationToken.None));
        }

        [Fact]
        public async Task DeleteAsync_ShouldThrowValidationException_WhenDeletingLastAdmin()
        {
            var repository = new FakeUserRepository();
            repository.Users.Add(new UserDomain
            {
                Id = 1,
                Email = "admin@test.com",
                Username = "admin",
                Password = "hashed",
                Role = UserRole.Admin
            });

            var service = new UserService(repository, TestMapperFactory.Create(), new FakeCurrentUserContext());

            await Assert.ThrowsAsync<ValidationException>(() => service.DeleteAsync(1, CancellationToken.None));
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrowValidationException_WhenUserChangesOwnRole()
        {
            var repository = new FakeUserRepository();
            repository.Users.Add(new UserDomain
            {
                Id = 7,
                Email = "user@test.com",
                Username = "user",
                Password = "hashed",
                Role = UserRole.User
            });

            var service = new UserService(
                repository,
                TestMapperFactory.Create(),
                new FakeCurrentUserContext { UserId = 7 });

            var dto = new UpdateUserDto
            {
                Email = "user@test.com",
                Username = "user",
                Role = UserRole.Admin
            };

            await Assert.ThrowsAsync<ValidationException>(() => service.UpdateAsync(7, dto, CancellationToken.None));
        }
    }
}
