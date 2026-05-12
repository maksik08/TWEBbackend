using ProjectBackend.api.Data;
using ProjectBackend.api.Models.Domain;

namespace ProjectBackend.api.Repositories
{
    public class ContactMessageRepository : IContactMessageRepository
    {
        private readonly ProjectDbContext _dbContext;

        public ContactMessageRepository(ProjectDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ContactMessageDomain> CreateAsync(ContactMessageDomain entity, CancellationToken cancellationToken)
        {
            await _dbContext.ContactMessages.AddAsync(entity, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return entity;
        }
    }
}
