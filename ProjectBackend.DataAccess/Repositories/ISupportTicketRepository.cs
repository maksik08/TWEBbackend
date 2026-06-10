using ProjectBackend.Domain.Common;
using ProjectBackend.Domain.Entities;
using ProjectBackend.Domain.Query;

namespace ProjectBackend.DataAccess.Repositories
{
    public interface ISupportTicketRepository
    {
        Task<SupportTicketDomain> CreateAsync(SupportTicketDomain ticket, CancellationToken cancellationToken);
        Task<PagedResult<SupportTicketDomain>> GetAllAsync(SupportTicketQueryOptions queryOptions, CancellationToken cancellationToken);
        Task<SupportTicketDomain?> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<SupportTicketDomain?> GetTrackedByIdAsync(int id, CancellationToken cancellationToken);
        Task<SupportTicketDomain> UpdateAsync(SupportTicketDomain ticket, CancellationToken cancellationToken);
        Task<SupportMessageDomain> AddMessageAsync(SupportMessageDomain message, CancellationToken cancellationToken);
        Task<SupportAttachmentDomain> AddAttachmentAsync(SupportAttachmentDomain attachment, CancellationToken cancellationToken);
    }
}
