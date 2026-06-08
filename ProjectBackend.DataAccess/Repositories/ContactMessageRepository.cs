using Microsoft.EntityFrameworkCore;
using ProjectBackend.DataAccess;
using ProjectBackend.Domain.Common;
using ProjectBackend.Domain.Entities;
using ProjectBackend.Domain.Query;

namespace ProjectBackend.DataAccess.Repositories
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

        public async Task<PagedResult<ContactMessageDomain>> GetAllAsync(ContactMessageQueryOptions queryOptions, CancellationToken cancellationToken)
        {
            var query = _dbContext.ContactMessages
                .AsNoTracking()
                .Include(message => message.User)
                .AsQueryable();

            if (queryOptions.Status.HasValue)
            {
                query = query.Where(message => message.Status == queryOptions.Status.Value);
            }

            if (!string.IsNullOrWhiteSpace(queryOptions.Search))
            {
                var search = queryOptions.Search;
                query = query.Where(message =>
                    message.Name.Contains(search) ||
                    message.Email.Contains(search) ||
                    message.Subject.Contains(search) ||
                    message.Message.Contains(search));
            }

            query = queryOptions.SortDescending
                ? query.OrderByDescending(message => message.CreatedAt).ThenByDescending(message => message.Id)
                : query.OrderBy(message => message.CreatedAt).ThenBy(message => message.Id);

            return await query.ToPagedResultAsync(queryOptions, cancellationToken);
        }

        public async Task<ContactMessageDomain?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await _dbContext.ContactMessages
                .AsNoTracking()
                .Include(message => message.User)
                .FirstOrDefaultAsync(message => message.Id == id, cancellationToken);
        }

        public async Task<ContactMessageDomain?> UpdateStatusAsync(int id, ContactMessageStatus status, CancellationToken cancellationToken)
        {
            var existing = await _dbContext.ContactMessages.FirstOrDefaultAsync(message => message.Id == id, cancellationToken);
            if (existing is null)
            {
                return null;
            }

            existing.Status = status;
            existing.IsRead = status != ContactMessageStatus.New;
            await _dbContext.SaveChangesAsync(cancellationToken);
            return existing;
        }
    }
}
