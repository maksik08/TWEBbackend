using Microsoft.EntityFrameworkCore;
using ProjectBackend.api.Data;
using ProjectBackend.api.Models.Domain;

namespace ProjectBackend.api.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ProjectDbContext _dbContext;

        public CategoryRepository(ProjectDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<CategoryDomain>> GetAllAsync()
        {
            return await _dbContext.Categories
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<CategoryDomain?> GetByIdAsync(int id)
        {
            return await _dbContext.Categories
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _dbContext.Categories.AnyAsync(c => c.Id == id);
        }

        public async Task<bool> HasProductsAsync(int id)
        {
            return await _dbContext.Products.AnyAsync(p => p.CategoryId == id);
        }

        public async Task<CategoryDomain> CreateAsync(CategoryDomain category)
        {
            await _dbContext.Categories.AddAsync(category);
            await _dbContext.SaveChangesAsync();
            return category;
        }

        public async Task<CategoryDomain?> UpdateAsync(int id, CategoryDomain category)
        {
            var existing = await _dbContext.Categories.FirstOrDefaultAsync(c => c.Id == id);
            if (existing is null) return null;

            existing.Name = category.Name;
            existing.Description = category.Description;

            await _dbContext.SaveChangesAsync();
            return existing;
        }

        public async Task<CategoryDomain?> DeleteAsync(int id)
        {
            var existing = await _dbContext.Categories.FirstOrDefaultAsync(c => c.Id == id);
            if (existing is null) return null;

            _dbContext.Categories.Remove(existing);
            await _dbContext.SaveChangesAsync();
            return existing;
        }
    }
}
