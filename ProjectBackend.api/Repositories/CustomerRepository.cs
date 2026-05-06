using Microsoft.EntityFrameworkCore;
using ProjectBackend.api.Data;
using ProjectBackend.api.Models.Common;
using ProjectBackend.api.Models.Domain;
using ProjectBackend.api.Models.Query;

namespace ProjectBackend.api.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly ProjectDbContext _dbContext;

        public CustomerRepository(ProjectDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PagedResult<CustomerDomain>> GetAllAsync(CustomerQueryOptions queryOptions, CancellationToken cancellationToken)
        {
            var query = _dbContext.Customers.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(queryOptions.Search))
            {
                query = query.Where(customer =>
                    customer.FirstName.Contains(queryOptions.Search) ||
                    customer.LastName.Contains(queryOptions.Search) ||
                    (customer.Email != null && customer.Email.Contains(queryOptions.Search)) ||
                    (customer.Phone != null && customer.Phone.Contains(queryOptions.Search)));
            }

            query = queryOptions.SortBy switch
            {
                "firstname" => queryOptions.SortDescending
                    ? query.OrderByDescending(customer => customer.FirstName).ThenBy(customer => customer.Id)
                    : query.OrderBy(customer => customer.FirstName).ThenBy(customer => customer.Id),
                "email" => queryOptions.SortDescending
                    ? query.OrderByDescending(customer => customer.Email).ThenBy(customer => customer.Id)
                    : query.OrderBy(customer => customer.Email).ThenBy(customer => customer.Id),
                _ => queryOptions.SortDescending
                    ? query.OrderByDescending(customer => customer.LastName).ThenBy(customer => customer.Id)
                    : query.OrderBy(customer => customer.LastName).ThenBy(customer => customer.Id)
            };

            return await query.ToPagedResultAsync(queryOptions, cancellationToken);
        }

        public async Task<CustomerDomain?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await _dbContext.Customers
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        }

        public async Task<CustomerDomain> CreateAsync(CustomerDomain customer, CancellationToken cancellationToken)
        {
            await _dbContext.Customers.AddAsync(customer, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return customer;
        }

        public async Task<CustomerDomain?> UpdateAsync(int id, CustomerDomain customer, CancellationToken cancellationToken)
        {
            var existing = await _dbContext.Customers.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
            if (existing is null) return null;

            existing.FirstName = customer.FirstName;
            existing.LastName = customer.LastName;
            existing.Email = customer.Email;
            existing.Phone = customer.Phone;

            await _dbContext.SaveChangesAsync(cancellationToken);
            return existing;
        }

        public async Task<CustomerDomain?> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            var existing = await _dbContext.Customers.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
            if (existing is null) return null;

            _dbContext.Customers.Remove(existing);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return existing;
        }
    }
}
