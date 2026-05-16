using System.Text;
using Microsoft.AspNetCore.Http;
using ProjectBackend.api.Exceptions;
using ProjectBackend.api.Models.Domain;
using ProjectBackend.api.Models.DTO;
using ProjectBackend.api.Services;
using ProjectBackend.Tests.TestInfrastructure;

namespace ProjectBackend.Tests.Services
{
    public class InstallerServiceTests
    {
        [Fact]
        public async Task CompleteRequestAsync_ShouldThrowValidationException_WhenMoreThanThreePhotosProvided()
        {
            var serviceRequestRepository = new FakeServiceRequestRepository();
            serviceRequestRepository.Requests.Add(new ServiceRequestDomain
            {
                Id = 1,
                RequestNumber = "REQ-2",
                CustomerId = 2,
                InstallerId = 4,
                ServiceTitle = "Installation",
                Address = "Address",
                ContactPhone = "555",
                Status = ServiceRequestStatus.InProgress
            });

            var service = new InstallerService(
                serviceRequestRepository,
                new FakeWorkPhotoStorageService(),
                new FakeNotificationService(),
                new FakeActionLogService(),
                new FakeCurrentUserContext { UserId = 4, Role = UserRole.Installer, Username = "installer" },
                TestMapperFactory.Create());

            var dto = new CompleteServiceRequestDto
            {
                Report = "Completed",
                Photos =
                [
                    CreateFormFile("1.jpg"),
                    CreateFormFile("2.jpg"),
                    CreateFormFile("3.jpg"),
                    CreateFormFile("4.jpg")
                ]
            };

            await Assert.ThrowsAsync<ValidationException>(() => service.CompleteRequestAsync(1, dto, CancellationToken.None));
        }

        private static IFormFile CreateFormFile(string fileName)
        {
            var bytes = Encoding.UTF8.GetBytes("test");
            var stream = new MemoryStream(bytes);
            return new FormFile(stream, 0, bytes.Length, "file", fileName);
        }
    }
}
