using ProjectBackend.Domain.Common;
using ProjectBackend.BusinessLogic.Dto;

namespace ProjectBackend.BusinessLogic.Services
{
    public interface ISupportTicketService
    {
        Task<SupportTicketDto> CreateAsync(CreateSupportTicketDto dto, CancellationToken cancellationToken);
        Task<PagedResult<SupportTicketDto>> GetMineAsync(SupportTicketListRequestDto request, CancellationToken cancellationToken);
        Task<PagedResult<SupportTicketDto>> GetAllAsync(SupportTicketListRequestDto request, CancellationToken cancellationToken);
        Task<SupportTicketDto> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<SupportTicketDto> PostMessageAsync(int id, PostSupportMessageDto dto, CancellationToken cancellationToken);
        Task<SupportTicketDto> UpdateStatusAsync(int id, UpdateSupportTicketStatusDto dto, CancellationToken cancellationToken);
        Task<SupportTicketDto> AssignAsync(int id, AssignSupportAgentDto dto, CancellationToken cancellationToken);
    }
}
