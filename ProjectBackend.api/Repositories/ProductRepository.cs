using Microsoft.EntityFrameworkCore;
using ProjectBackend.api.Data;
using ProjectBackend.api.Models.Domain;

namespace ProjectBackend.api.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ProjectDbContext _dbContext;

        public ProductRepository(ProjectDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<ProductsDomain>> GetAllAsync()
        {
            return await _dbContext.Products
                .AsNoTracking()
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .ToListAsync();
        }

        public async Task<ProductsDomain?> GetByIdAsync(int id)
        {
            return await _dbContext.Products
                .AsNoTracking()
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<ProductsDomain> CreateAsync(ProductsDomain product)
        {
            await _dbContext.Products.AddAsync(product);
            await _dbContext.SaveChangesAsync();
            return product;
        }

        public async Task<ProductsDomain?> UpdateAsync(int id, ProductsDomain product)
        {
            var existing = await _dbContext.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (existing is null) return null;

            existing.Name = product.Name;
            existing.Title = product.Title;
            existing.Image = product.Image;
            existing.Price = product.Price;
            existing.CategoryId = product.CategoryId;
            existing.SupplierId = product.SupplierId;

            await _dbContext.SaveChangesAsync();

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

        public async Task<ProductsDomain?> DeleteAsync(int id)
        {
            var existing = await _dbContext.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (existing is null) return null;

            _dbContext.Products.Remove(existing);
            await _dbContext.SaveChangesAsync();
            return existing;
        }
    }
}
