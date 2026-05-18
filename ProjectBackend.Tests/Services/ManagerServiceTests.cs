using ProjectBackend.api.Models.Domain;
using ProjectBackend.api.Models.DTO;
using ProjectBackend.api.Services;
using ProjectBackend.Tests.TestInfrastructure;

namespace ProjectBackend.Tests.Services
{
    public class ManagerServiceTests
    {
        [Fact]
        public async Task AssignInstallerAsync_ShouldAssignInstallerAndNotifyInstaller()
        {
            var orderRepository = new FakeOrderRepository();
            var serviceRequestRepository = new FakeServiceRequestRepository();
            var userRepository = new FakeUserRepository();
            var productRepository = new FakeProductRepository();
            var notificationService = new FakeNotificationService();
            var actionLogService = new FakeActionLogService();

            userRepository.Users.Add(new UserDomain
            {
                Id = 5,
                Email = "installer@test.com",
                Username = "installer",
                Password = "hashed",
                Role = UserRole.Installer
            });

            serviceRequestRepository.Requests.Add(new ServiceRequestDomain
            {
                Id = 1,
                RequestNumber = "REQ-1",
                CustomerId = 2,
                ServiceTitle = "Fiber setup",
                Address = "Customer street 10",
                ContactPhone = "555",
                Status = ServiceRequestStatus.Submitted
            });

            var service = new ManagerService(
                orderRepository,
                serviceRequestRepository,
                userRepository,
                productRepository,
                notificationService,
                actionLogService,
                new FakeCurrentUserContext { UserId = 11, Role = UserRole.Manager, Username = "manager" },
                TestMapperFactory.Create());

            var updated = await service.AssignInstallerAsync(1, new AssignInstallerDto { InstallerId = 5 }, CancellationToken.None);

            Assert.Equal(ServiceRequestStatus.Assigned, updated.Status);
            Assert.Equal(5, updated.InstallerId);
            Assert.Equal(11, updated.ManagerId);
            Assert.Single(notificationService.Notifications);
            Assert.Single(actionLogService.Entries);
        }
    }
}
