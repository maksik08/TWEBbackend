using ProjectBackend.api.Models.Common;
using ProjectBackend.api.Models.DTO;

namespace ProjectBackend.api.Services
{
    public interface IServiceRequestService
    {
        Task<PagedResult<ServiceRequestDto>> GetMyRequestsAsync(ServiceRequestListRequestDto request, CancellationToken cancellationToken);
        Task<ServiceRequestDto> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<ServiceRequestDto> CreateAsync(CreateServiceRequestDto dto, CancellationToken cancellationToken);
    }
}
