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
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .ToListAsync();
        }

        public async Task<ProductsDomain?> GetByIdAsync(int id)
        {
            return await _dbContext.Products
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
            var existing = await _dbContext.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (existing is null) return null;

            existing.Name = product.Name;
            existing.Title = product.Title;
            existing.Image = product.Image;
            existing.Price = product.Price;
            existing.CategoryId = product.CategoryId;
            existing.SupplierId = product.SupplierId;

            await _dbContext.SaveChangesAsync();

            if (existing.CategoryId != null && existing.Category?.Id != existing.CategoryId)
            {
                await _dbContext.Entry(existing).Reference(p => p.Category).LoadAsync();
            }

            return existing;
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
