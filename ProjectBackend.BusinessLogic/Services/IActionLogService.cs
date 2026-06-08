using ProjectBackend.Domain.Common;
using ProjectBackend.BusinessLogic.Dto;

namespace ProjectBackend.BusinessLogic.Services
{
    public interface IActionLogService
    {
        Task RecordAsync(string action, string entityType, int? entityId, string? details, CancellationToken cancellationToken);

        Task<PagedResult<ActionLogDto>> GetAllAsync(ActionLogListRequestDto request, CancellationToken cancellationToken);
    }
}
