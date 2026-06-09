using ProjectBackend.Domain.Common;
using ProjectBackend.BusinessLogic.Dto;

namespace ProjectBackend.BusinessLogic.Services
{
    public interface IServiceRequestService
    {
        Task<PagedResult<ServiceRequestDto>> GetMyRequestsAsync(ServiceRequestListRequestDto request, CancellationToken cancellationToken);
        Task<ServiceRequestDto> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<ServiceRequestDto> CreateAsync(CreateServiceRequestDto dto, CancellationToken cancellationToken);
        Task<ServiceRequestDto> PayAsync(int id, CancellationToken cancellationToken);
        Task<ServiceRequestDto> RateAsync(int id, RateServiceRequestDto dto, CancellationToken cancellationToken);
    }
}
