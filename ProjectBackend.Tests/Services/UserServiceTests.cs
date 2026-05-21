using ProjectBackend.BusinessLogic.Exceptions;
using ProjectBackend.Domain.Entities;
using ProjectBackend.BusinessLogic.Dto;
using ProjectBackend.BusinessLogic.Services;
using ProjectBackend.Tests.TestInfrastructure;

namespace ProjectBackend.Tests.Services
{
    public class UserServiceTests
    {
        [Fact]
        public async Task CreateAsync_ShouldThrowConflictException_WhenEmailAlreadyExists()
        {
            var repository = new FakeUserRepository();
            var paymentTransactionService = new FakePaymentTransactionService();
            repository.Users.Add(new UserDomain
            {
                Id = 1,
                Email = "existing@test.com",
                Username = "existing",
                Password = "hashed",
                Role = UserRole.User
            });

            var service = new UserService(repository, paymentTransactionService, TestMapperFactory.Create(), new FakeCurrentUserContext());

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
            var service = new UserService(new FakeUserRepository(), new FakePaymentTransactionService(), TestMapperFactory.Create(), new FakeCurrentUserContext());

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

            var service = new UserService(repository, new FakePaymentTransactionService(), TestMapperFactory.Create(), new FakeCurrentUserContext());

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
                new FakePaymentTransactionService(),
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

        [Fact]
        public async Task TopUpBalanceAsync_ShouldRecordPaymentTransaction()
        {
            var repository = new FakeUserRepository();
            var paymentTransactionService = new FakePaymentTransactionService();
            repository.Users.Add(new UserDomain
            {
                Id = 4,
                Email = "user@test.com",
                Username = "user",
                Password = "hashed",
                Role = UserRole.User,
                Balance = 10
            });

            var service = new UserService(
                repository,
                paymentTransactionService,
                TestMapperFactory.Create(),
                new FakeCurrentUserContext { UserId = 4, Role = UserRole.User });

            var updated = await service.TopUpBalanceAsync(4, 25, CancellationToken.None);

            Assert.Equal(35, updated.Balance);
            Assert.Single(paymentTransactionService.Payments);
            Assert.Equal(PaymentTransactionType.BalanceTopUp, paymentTransactionService.Payments[0].Type);
            Assert.Equal(25, paymentTransactionService.Payments[0].Amount);
        }
    }
}
