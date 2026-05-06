using Microsoft.EntityFrameworkCore;
using ProjectBackend.api.Data;
using ProjectBackend.api.Models.Common;
using ProjectBackend.api.Models.Domain;
using ProjectBackend.api.Models.Query;

namespace ProjectBackend.api.Repositories
{
    public class SupplierRepository : ISupplierRepository
    {
        private readonly ProjectDbContext _dbContext;

        public SupplierRepository(ProjectDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PagedResult<SupplierDomain>> GetAllAsync(SupplierQueryOptions queryOptions, CancellationToken cancellationToken)
        {
            var query = _dbContext.Suppliers.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(queryOptions.Search))
            {
                query = query.Where(supplier =>
                    supplier.Name.Contains(queryOptions.Search) ||
                    (supplier.ContactEmail != null && supplier.ContactEmail.Contains(queryOptions.Search)) ||
                    (supplier.Phone != null && supplier.Phone.Contains(queryOptions.Search)));
            }

            query = queryOptions.SortBy switch
            {
                "contactemail" => queryOptions.SortDescending
                    ? query.OrderByDescending(supplier => supplier.ContactEmail).ThenBy(supplier => supplier.Id)
                    : query.OrderBy(supplier => supplier.ContactEmail).ThenBy(supplier => supplier.Id),
                _ => queryOptions.SortDescending
                    ? query.OrderByDescending(supplier => supplier.Name).ThenBy(supplier => supplier.Id)
                    : query.OrderBy(supplier => supplier.Name).ThenBy(supplier => supplier.Id)
            };

            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .Skip(queryOptions.Skip)
                .Take(queryOptions.PageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<SupplierDomain>
            {
                Items = items,
                TotalCount = totalCount,
                Page = queryOptions.Page,
                PageSize = queryOptions.PageSize
            };
        }

        public async Task<SupplierDomain?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await _dbContext.Suppliers
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
        }

        public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken)
        {
            return await _dbContext.Suppliers.AnyAsync(s => s.Id == id, cancellationToken);
        }

        public async Task<bool> HasProductsAsync(int id, CancellationToken cancellationToken)
        {
            return await _dbContext.Products.AnyAsync(p => p.SupplierId == id, cancellationToken);
        }

        public async Task<SupplierDomain> CreateAsync(SupplierDomain supplier, CancellationToken cancellationToken)
        {
            await _dbContext.Suppliers.AddAsync(supplier, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return supplier;
        }

        public async Task<SupplierDomain?> UpdateAsync(int id, SupplierDomain supplier, CancellationToken cancellationToken)
        {
            var existing = await _dbContext.Suppliers.FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
            if (existing is null) return null;

            existing.Name = supplier.Name;
            existing.ContactEmail = supplier.ContactEmail;
            existing.Phone = supplier.Phone;

            await _dbContext.SaveChangesAsync(cancellationToken);
            return existing;
        }

        public async Task<SupplierDomain?> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            var existing = await _dbContext.Suppliers.FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
            if (existing is null) return null;

            _dbContext.Suppliers.Remove(existing);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return existing;
        }
    }
}
