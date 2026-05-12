using ProjectBackend.api.Models.Domain;

namespace ProjectBackend.api.Repositories
{
    public interface IContactMessageRepository
    {
        Task<ContactMessageDomain> CreateAsync(ContactMessageDomain entity, CancellationToken cancellationToken);
    }
}
