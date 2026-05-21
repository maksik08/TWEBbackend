using ProjectBackend.Domain.Entities;

namespace ProjectBackend.DataAccess.Repositories
{
    public interface IContactMessageRepository
    {
        Task<ContactMessageDomain> CreateAsync(ContactMessageDomain entity, CancellationToken cancellationToken);
    }
}
