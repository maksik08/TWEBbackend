using Microsoft.EntityFrameworkCore;
using ProjectBackend.DataAccess;
using ProjectBackend.Domain.Common;
using ProjectBackend.Domain.Entities;
using ProjectBackend.Domain.Query;

namespace ProjectBackend.DataAccess.Repositories
{
    public class SupportTicketRepository : ISupportTicketRepository
    {
        private readonly ProjectDbContext _dbContext;

        public SupportTicketRepository(ProjectDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<SupportTicketDomain> CreateAsync(SupportTicketDomain ticket, CancellationToken cancellationToken)
        {
            await _dbContext.SupportTickets.AddAsync(ticket, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return ticket;
        }

        public async Task<PagedResult<SupportTicketDomain>> GetAllAsync(SupportTicketQueryOptions queryOptions, CancellationToken cancellationToken)
        {
            var query = _dbContext.SupportTickets
                .AsNoTracking()
                .Include(ticket => ticket.Customer)
                .Include(ticket => ticket.AssignedAgent)
                .AsQueryable();

            if (queryOptions.Status.HasValue)
            {
                query = query.Where(ticket => ticket.Status == queryOptions.Status.Value);
            }

            if (queryOptions.Priority.HasValue)
            {
                query = query.Where(ticket => ticket.Priority == queryOptions.Priority.Value);
            }

            if (queryOptions.CustomerId.HasValue)
            {
                query = query.Where(ticket => ticket.CustomerId == queryOptions.CustomerId.Value);
            }

            if (queryOptions.AssignedAgentId.HasValue)
            {
                query = query.Where(ticket => ticket.AssignedAgentId == queryOptions.AssignedAgentId.Value);
            }

            if (!string.IsNullOrWhiteSpace(queryOptions.Search))
            {
                var search = queryOptions.Search;
                query = query.Where(ticket => ticket.Subject.Contains(search));
            }

            query = queryOptions.SortDescending
                ? query.OrderByDescending(ticket => ticket.UpdatedAt).ThenByDescending(ticket => ticket.Id)
                : query.OrderBy(ticket => ticket.UpdatedAt).ThenBy(ticket => ticket.Id);

            return await query.ToPagedResultAsync(queryOptions, cancellationToken);
        }

        public async Task<SupportTicketDomain?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await _dbContext.SupportTickets
                .AsNoTracking()
                .Include(ticket => ticket.Customer)
                .Include(ticket => ticket.AssignedAgent)
                .Include(ticket => ticket.Attachments)
                    .ThenInclude(attachment => attachment.UploadedByUser)
                .Include(ticket => ticket.Messages.OrderBy(message => message.CreatedAt))
                    .ThenInclude(message => message.AuthorUser)
                .FirstOrDefaultAsync(ticket => ticket.Id == id, cancellationToken);
        }

        public async Task<SupportTicketDomain?> GetTrackedByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await _dbContext.SupportTickets
                .FirstOrDefaultAsync(ticket => ticket.Id == id, cancellationToken);
        }

        public async Task<SupportTicketDomain> UpdateAsync(SupportTicketDomain ticket, CancellationToken cancellationToken)
        {
            ticket.UpdatedAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync(cancellationToken);
            return ticket;
        }

        public async Task<SupportMessageDomain> AddMessageAsync(SupportMessageDomain message, CancellationToken cancellationToken)
        {
            await _dbContext.SupportMessages.AddAsync(message, cancellationToken);

            var ticket = await _dbContext.SupportTickets.FirstOrDefaultAsync(t => t.Id == message.TicketId, cancellationToken);
            if (ticket is not null)
            {
                ticket.UpdatedAt = DateTime.UtcNow;
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
            return message;
        }

        public async Task<SupportAttachmentDomain> AddAttachmentAsync(SupportAttachmentDomain attachment, CancellationToken cancellationToken)
        {
            await _dbContext.SupportAttachments.AddAsync(attachment, cancellationToken);

            var ticket = await _dbContext.SupportTickets.FirstOrDefaultAsync(t => t.Id == attachment.TicketId, cancellationToken);
            if (ticket is not null)
            {
                ticket.UpdatedAt = DateTime.UtcNow;
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
            await _dbContext.Entry(attachment).Reference(item => item.UploadedByUser).LoadAsync(cancellationToken);
            return attachment;
        }
    }
}
