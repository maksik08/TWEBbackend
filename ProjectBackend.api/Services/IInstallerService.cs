using ProjectBackend.api.Models.Common;
using ProjectBackend.api.Models.DTO;

namespace ProjectBackend.api.Services
{
    public interface IInstallerService
    {
        Task<PagedResult<ServiceRequestDto>> GetMyRequestsAsync(ServiceRequestListRequestDto request, CancellationToken cancellationToken);
        Task<IReadOnlyCollection<ServiceRequestDto>> GetRouteAsync(DateTime? date, CancellationToken cancellationToken);
        Task<ServiceRequestDto> StartRequestAsync(int id, CancellationToken cancellationToken);
        Task<ServiceRequestDto> CompleteRequestAsync(int id, CompleteServiceRequestDto dto, CancellationToken cancellationToken);
    }
}
