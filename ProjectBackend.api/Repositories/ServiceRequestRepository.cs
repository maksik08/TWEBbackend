using Microsoft.EntityFrameworkCore;
using ProjectBackend.api.Data;
using ProjectBackend.api.Models.Common;
using ProjectBackend.api.Models.Domain;
using ProjectBackend.api.Models.Query;

namespace ProjectBackend.api.Repositories
{
    public class ServiceRequestRepository : IServiceRequestRepository
    {
        private readonly ProjectDbContext _dbContext;

        public ServiceRequestRepository(ProjectDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PagedResult<ServiceRequestDomain>> GetAllAsync(
            ServiceRequestQueryOptions queryOptions,
            CancellationToken cancellationToken)
        {
            var query = BuildQuery(tracked: false);

            if (!string.IsNullOrWhiteSpace(queryOptions.Search))
            {
                query = query.Where(request =>
                    request.RequestNumber.Contains(queryOptions.Search) ||
                    request.ServiceTitle.Contains(queryOptions.Search) ||
                    request.Address.Contains(queryOptions.Search) ||
                    request.Customer!.Username.Contains(queryOptions.Search));
            }

            if (queryOptions.Status.HasValue)
            {
                query = query.Where(request => request.Status == queryOptions.Status.Value);
            }

            if (queryOptions.CustomerId.HasValue)
            {
                query = query.Where(request => request.CustomerId == queryOptions.CustomerId.Value);
            }

            if (queryOptions.InstallerId.HasValue)
            {
                query = query.Where(request => request.InstallerId == queryOptions.InstallerId.Value);
            }

            if (queryOptions.ScheduledFrom.HasValue)
            {
                query = query.Where(request => request.ScheduledVisitAt >= queryOptions.ScheduledFrom.Value);
            }

            if (queryOptions.ScheduledTo.HasValue)
            {
                query = query.Where(request => request.ScheduledVisitAt <= queryOptions.ScheduledTo.Value);
            }

            query = queryOptions.SortBy switch
            {
                "scheduledvisitat" => queryOptions.SortDescending
                    ? query.OrderByDescending(request => request.ScheduledVisitAt).ThenByDescending(request => request.Id)
                    : query.OrderBy(request => request.ScheduledVisitAt).ThenBy(request => request.Id),
                "address" => queryOptions.SortDescending
                    ? query.OrderByDescending(request => request.Address).ThenByDescending(request => request.Id)
                    : query.OrderBy(request => request.Address).ThenBy(request => request.Id),
                _ => queryOptions.SortDescending
                    ? query.OrderByDescending(request => request.CreatedAt).ThenByDescending(request => request.Id)
                    : query.OrderBy(request => request.CreatedAt).ThenBy(request => request.Id)
            };

            return await query.ToPagedResultAsync(queryOptions, cancellationToken);
        }

        public async Task<ServiceRequestDomain?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await BuildQuery(tracked: false)
                .FirstOrDefaultAsync(request => request.Id == id, cancellationToken);
        }

        public async Task<ServiceRequestDomain?> GetTrackedByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await BuildQuery(tracked: true)
                .FirstOrDefaultAsync(request => request.Id == id, cancellationToken);
        }

        public async Task<ServiceRequestDomain> CreateAsync(ServiceRequestDomain serviceRequest, CancellationToken cancellationToken)
        {
            await _dbContext.ServiceRequests.AddAsync(serviceRequest, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return await GetTrackedByIdAsync(serviceRequest.Id, cancellationToken) ?? serviceRequest;
        }

        public async Task<ServiceRequestDomain> UpdateAsync(ServiceRequestDomain serviceRequest, CancellationToken cancellationToken)
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
            return await GetTrackedByIdAsync(serviceRequest.Id, cancellationToken) ?? serviceRequest;
        }

        public async Task<ServiceRequestDomain?> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            var existing = await _dbContext.ServiceRequests.FirstOrDefaultAsync(request => request.Id == id, cancellationToken);
            if (existing is null)
            {
                return null;
            }

            _dbContext.ServiceRequests.Remove(existing);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return existing;
        }

        public async Task<IReadOnlyCollection<ServiceRequestDomain>> GetAllForReportAsync(CancellationToken cancellationToken)
        {
            return await BuildQuery(tracked: false)
                .OrderByDescending(request => request.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        private IQueryable<ServiceRequestDomain> BuildQuery(bool tracked)
        {
            var query = tracked
                ? _dbContext.ServiceRequests.AsQueryable()
                : _dbContext.ServiceRequests.AsNoTracking();

            return query
                .Include(request => request.Customer)
                .Include(request => request.Installer)
                .Include(request => request.Manager)
                .Include(request => request.Comments)
                    .ThenInclude(comment => comment.AuthorUser)
                .Include(request => request.WorkPhotos);
        }
    }
}
