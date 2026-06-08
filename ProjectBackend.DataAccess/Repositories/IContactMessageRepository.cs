using ProjectBackend.Domain.Common;
using ProjectBackend.Domain.Entities;
using ProjectBackend.Domain.Query;

namespace ProjectBackend.DataAccess.Repositories
{
    public interface IContactMessageRepository
    {
        Task<ContactMessageDomain> CreateAsync(ContactMessageDomain entity, CancellationToken cancellationToken);
        Task<PagedResult<ContactMessageDomain>> GetAllAsync(ContactMessageQueryOptions queryOptions, CancellationToken cancellationToken);
        Task<ContactMessageDomain?> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<ContactMessageDomain?> UpdateStatusAsync(int id, ContactMessageStatus status, CancellationToken cancellationToken);
    }
}
