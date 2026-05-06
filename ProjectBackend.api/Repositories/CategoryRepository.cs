using Microsoft.EntityFrameworkCore;
using ProjectBackend.api.Models.Common;
using ProjectBackend.api.Data;
using ProjectBackend.api.Models.Domain;
using ProjectBackend.api.Models.Query;

namespace ProjectBackend.api.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ProjectDbContext _dbContext;

        public CategoryRepository(ProjectDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PagedResult<CategoryDomain>> GetAllAsync(CategoryQueryOptions queryOptions, CancellationToken cancellationToken)
        {
            var query = _dbContext.Categories.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(queryOptions.Search))
            {
                query = query.Where(category =>
                    category.Name.Contains(queryOptions.Search) ||
                    (category.Description != null && category.Description.Contains(queryOptions.Search)));
            }

            query = queryOptions.SortBy switch
            {
                "description" => queryOptions.SortDescending
                    ? query.OrderByDescending(category => category.Description).ThenBy(category => category.Id)
                    : query.OrderBy(category => category.Description).ThenBy(category => category.Id),
                _ => queryOptions.SortDescending
                    ? query.OrderByDescending(category => category.Name).ThenBy(category => category.Id)
                    : query.OrderBy(category => category.Name).ThenBy(category => category.Id)
            };

            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .Skip(queryOptions.Skip)
                .Take(queryOptions.PageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<CategoryDomain>
            {
                Items = items,
                TotalCount = totalCount,
                Page = queryOptions.Page,
                PageSize = queryOptions.PageSize
            };
        }

        public async Task<CategoryDomain?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await _dbContext.Categories
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        }

        public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken)
        {
            return await _dbContext.Categories.AnyAsync(c => c.Id == id, cancellationToken);
        }

        public async Task<bool> HasProductsAsync(int id, CancellationToken cancellationToken)
        {
            return await _dbContext.Products.AnyAsync(p => p.CategoryId == id, cancellationToken);
        }

        public async Task<CategoryDomain> CreateAsync(CategoryDomain category, CancellationToken cancellationToken)
        {
            await _dbContext.Categories.AddAsync(category, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return category;
        }

        public async Task<CategoryDomain?> UpdateAsync(int id, CategoryDomain category, CancellationToken cancellationToken)
        {
            var existing = await _dbContext.Categories.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
            if (existing is null) return null;

            existing.Name = category.Name;
            existing.Description = category.Description;

            await _dbContext.SaveChangesAsync(cancellationToken);
            return existing;
        }

        public async Task<CategoryDomain?> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            var existing = await _dbContext.Categories.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
            if (existing is null) return null;

            _dbContext.Categories.Remove(existing);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return existing;
        }
    }
}
