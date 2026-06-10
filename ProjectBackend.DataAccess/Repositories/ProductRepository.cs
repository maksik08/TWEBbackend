using Microsoft.EntityFrameworkCore;
using ProjectBackend.DataAccess;
using ProjectBackend.Domain.Common;
using ProjectBackend.Domain.Entities;
using ProjectBackend.Domain.Query;

namespace ProjectBackend.DataAccess.Repositories
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
                .Include(p => p.WarehouseZone)
                .AsQueryable();

            if (!queryOptions.IncludeHidden)
            {
                query = query.Where(product => product.IsVisible);
            }

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

            var page = await query.ToPagedResultAsync(queryOptions, cancellationToken);
            await EnrichWithRatingsAsync(page.Items, cancellationToken);
            return page;
        }

        public async Task<IReadOnlyCollection<ProductsDomain>> GetByIdsAsync(
            IReadOnlyCollection<int> ids,
            bool includeHidden,
            CancellationToken cancellationToken)
        {
            var query = _dbContext.Products
                .AsNoTracking()
                .Where(product => ids.Contains(product.Id));

            if (!includeHidden)
            {
                query = query.Where(product => product.IsVisible);
            }

            return await query.ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyCollection<ProductsDomain>> GetTrackedByIdsAsync(
            IReadOnlyCollection<int> ids,
            CancellationToken cancellationToken)
        {
            return await _dbContext.Products
                .Where(product => ids.Contains(product.Id))
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyCollection<ProductsDomain>> GetAllForStockReportAsync(CancellationToken cancellationToken)
        {
            return await _dbContext.Products
                .AsNoTracking()
                .Include(product => product.Category)
                .Include(product => product.Supplier)
                .Include(product => product.WarehouseZone)
                .OrderBy(product => product.Name)
                .ToListAsync(cancellationToken);
        }

        public async Task UpdateStockLevelsAsync(IReadOnlyDictionary<int, int> stockByProductId, CancellationToken cancellationToken)
        {
            if (stockByProductId.Count == 0)
            {
                return;
            }

            var ids = stockByProductId.Keys.ToArray();
            var products = await _dbContext.Products
                .Where(product => ids.Contains(product.Id))
                .ToListAsync(cancellationToken);

            foreach (var product in products)
            {
                if (stockByProductId.TryGetValue(product.Id, out var quantity))
                {
                    product.StockQuantity = quantity;
                }
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<ProductsDomain?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            var product = await _dbContext.Products
                .AsNoTracking()
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .Include(p => p.WarehouseZone)
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

            if (product is not null)
            {
                await EnrichWithRatingsAsync(new[] { product }, cancellationToken);
            }

            return product;
        }

        private async Task EnrichWithRatingsAsync(
            IReadOnlyCollection<ProductsDomain> products,
            CancellationToken cancellationToken)
        {
            if (products.Count == 0) return;

            var ids = products.Select(p => p.Id).ToList();
            var stats = await _dbContext.ProductReviews
                .Where(review => ids.Contains(review.ProductId))
                .GroupBy(review => review.ProductId)
                .Select(group => new
                {
                    ProductId = group.Key,
                    Average = group.Average(review => (double)review.Rating),
                    Count = group.Count(),
                })
                .ToDictionaryAsync(entry => entry.ProductId, cancellationToken);

            foreach (var product in products)
            {
                if (stats.TryGetValue(product.Id, out var entry))
                {
                    product.RatingAverage = entry.Average;
                    product.RatingCount = entry.Count;
                }
            }
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
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
            if (existing is null) return null;

            existing.Name = product.Name;
            existing.Title = product.Title;
            existing.Image = product.Image;
            existing.Brand = product.Brand;
            existing.Sku = product.Sku;
            existing.ShortDescription = product.ShortDescription;
            existing.Description = product.Description;
            existing.Warranty = product.Warranty;
            existing.Availability = product.Availability;
            existing.Technology = product.Technology;
            existing.KeyFeatures = product.KeyFeatures;
            existing.PackageContents = product.PackageContents;
            existing.Specifications = product.Specifications;
            existing.Price = product.Price;
            existing.StockQuantity = product.StockQuantity;
            existing.ReservedQuantity = product.ReservedQuantity;
            existing.MinStockLevel = product.MinStockLevel;
            existing.MaxStockLevel = product.MaxStockLevel;
            existing.IsPreorder = product.IsPreorder;
            existing.IsVisible = product.IsVisible;
            existing.CategoryId = product.CategoryId;
            existing.SupplierId = product.SupplierId;
            existing.WarehouseZoneId = product.WarehouseZoneId;

            await _dbContext.SaveChangesAsync(cancellationToken);

            await _dbContext.Entry(existing).Reference(p => p.Category).LoadAsync(cancellationToken);
            await _dbContext.Entry(existing).Reference(p => p.Supplier).LoadAsync(cancellationToken);
            await _dbContext.Entry(existing).Reference(p => p.WarehouseZone).LoadAsync(cancellationToken);

            return existing;
        }

        public async Task<ProductsDomain?> SetVisibilityAsync(int id, bool isVisible, CancellationToken cancellationToken)
        {
            var existing = await _dbContext.Products
                .Include(product => product.Category)
                .Include(product => product.Supplier)
                .Include(product => product.WarehouseZone)
                .FirstOrDefaultAsync(product => product.Id == id, cancellationToken);

            if (existing is null)
            {
                return null;
            }

            existing.IsVisible = isVisible;
            await _dbContext.SaveChangesAsync(cancellationToken);
            return existing;
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
