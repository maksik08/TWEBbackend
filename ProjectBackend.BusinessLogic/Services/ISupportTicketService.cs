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
        Task<SupportTicketDto> EscalateAsync(int id, EscalateSupportTicketDto dto, CancellationToken cancellationToken);
        Task<SupportTicketDto> RateAsync(int id, RateSupportTicketDto dto, CancellationToken cancellationToken);
        Task<SupportAttachmentDto> AddAttachmentAsync(int id, UploadSupportAttachmentDto dto, CancellationToken cancellationToken);
    }
}
