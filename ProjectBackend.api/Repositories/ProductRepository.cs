using Microsoft.EntityFrameworkCore;
using ProjectBackend.api.Data;
using ProjectBackend.api.Models.Common;
using ProjectBackend.api.Models.Domain;
using ProjectBackend.api.Models.Query;

namespace ProjectBackend.api.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ProjectDbContext _dbContext;

        public ProductRepository(ProjectDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PagedResult<ProductsDomain>> GetAllAsync(ProductQueryOptions queryOptions, CancellationToken cancellationToken)
        {
            var query = _dbContext.Products
                .AsNoTracking()
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(queryOptions.Search))
            {
                query = query.Where(product =>
                    product.Name.Contains(queryOptions.Search) ||
                    (product.Title != null && product.Title.Contains(queryOptions.Search)) ||
                    (product.Category != null && product.Category.Name.Contains(queryOptions.Search)) ||
                    (product.Supplier != null && product.Supplier.Name.Contains(queryOptions.Search)));
            }

            if (queryOptions.CategoryId.HasValue)
            {
                query = query.Where(product => product.CategoryId == queryOptions.CategoryId.Value);
            }

            if (queryOptions.SupplierId.HasValue)
            {
                query = query.Where(product => product.SupplierId == queryOptions.SupplierId.Value);
            }

            if (queryOptions.MinPrice.HasValue)
            {
                query = query.Where(product => product.Price >= queryOptions.MinPrice.Value);
            }

            if (queryOptions.MaxPrice.HasValue)
            {
                query = query.Where(product => product.Price <= queryOptions.MaxPrice.Value);
            }

            query = queryOptions.SortBy switch
            {
                "title" => queryOptions.SortDescending
                    ? query.OrderByDescending(product => product.Title).ThenBy(product => product.Id)
                    : query.OrderBy(product => product.Title).ThenBy(product => product.Id),
                "price" => queryOptions.SortDescending
                    ? query.OrderByDescending(product => product.Price).ThenBy(product => product.Id)
                    : query.OrderBy(product => product.Price).ThenBy(product => product.Id),
                _ => queryOptions.SortDescending
                    ? query.OrderByDescending(product => product.Name).ThenBy(product => product.Id)
                    : query.OrderBy(product => product.Name).ThenBy(product => product.Id)
            };

            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .Skip(queryOptions.Skip)
                .Take(queryOptions.PageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<ProductsDomain>
            {
                Items = items,
                TotalCount = totalCount,
                Page = queryOptions.Page,
                PageSize = queryOptions.PageSize
            };
        }

        public async Task<ProductsDomain?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await _dbContext.Products
                .AsNoTracking()
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        }

        public async Task<ProductsDomain> CreateAsync(ProductsDomain product, CancellationToken cancellationToken)
        {
            await _dbContext.Products.AddAsync(product, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return product;
        }

        public async Task<ProductsDomain?> UpdateAsync(int id, ProductsDomain product, CancellationToken cancellationToken)
        {
            var existing = await _dbContext.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
            if (existing is null) return null;

            existing.Name = product.Name;
            existing.Title = product.Title;
            existing.Image = product.Image;
            existing.Price = product.Price;
            existing.CategoryId = product.CategoryId;
            existing.SupplierId = product.SupplierId;

            await _dbContext.SaveChangesAsync(cancellationToken);

            await ReloadReferenceAsync(existing, p => p.Category, existing.CategoryId.HasValue);
            await ReloadReferenceAsync(existing, p => p.Supplier, existing.SupplierId.HasValue);

            return existing;
        }

        private async Task ReloadReferenceAsync<TProperty>(
            ProductsDomain entity,
            System.Linq.Expressions.Expression<Func<ProductsDomain, TProperty?>> navigation,
            bool hasForeignKey)
            where TProperty : class
        {
            var reference = _dbContext.Entry(entity).Reference(navigation);
            if (!hasForeignKey)
            {
                reference.CurrentValue = null;
                return;
            }

            reference.IsLoaded = false;
            await reference.LoadAsync();
        }

        public async Task<ProductsDomain?> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            var existing = await _dbContext.Products.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
            if (existing is null) return null;

            _dbContext.Products.Remove(existing);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return existing;
        }
    }
}
