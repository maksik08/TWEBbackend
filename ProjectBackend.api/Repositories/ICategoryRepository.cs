using ProjectBackend.api.Models.Domain;

namespace ProjectBackend.api.Repositories
{
    public interface ICategoryRepository
    {
        Task<List<CategoryDomain>> GetAllAsync();
        Task<CategoryDomain?> GetByIdAsync(int id);
        Task<CategoryDomain> CreateAsync(CategoryDomain category);
        Task<CategoryDomain?> UpdateAsync(int id, CategoryDomain category);
        Task<CategoryDomain?> DeleteAsync(int id);
    }
}
