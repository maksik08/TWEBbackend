using ProjectBackend.Domain.Common;
using ProjectBackend.BusinessLogic.Dto;

namespace ProjectBackend.BusinessLogic.Services
{
    public interface IReturnService
    {
        Task<PagedResult<ReturnDto>> GetAllAsync(ReturnListRequestDto request, CancellationToken cancellationToken);
        Task<ReturnDto> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<ReturnDto> CreateAsync(CreateReturnDto dto, CancellationToken cancellationToken);
        Task<ReturnDto> ApproveAsync(int id, ResolveReturnDto dto, CancellationToken cancellationToken);
        Task<ReturnDto> RejectAsync(int id, ResolveReturnDto dto, CancellationToken cancellationToken);
    }
}
