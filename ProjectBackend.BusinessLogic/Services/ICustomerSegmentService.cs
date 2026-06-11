using ProjectBackend.BusinessLogic.Dto;
using ProjectBackend.Domain.Common;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectBackend.BusinessLogic.Services
{
    public interface ICustomerSegmentService
    {
        Task<PagedResult<CustomerSegmentDto>> GetAllAsync(int page, int pageSize, CancellationToken ct);
        Task<CustomerSegmentDto> CreateAsync(CreateCustomerSegmentDto dto, CancellationToken ct);
        Task AssignAsync(int id, int customerId, CancellationToken ct);
    }
}