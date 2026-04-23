using ProjectBackend.api.Models.DTO;

namespace ProjectBackend.api.Services
{
    public interface ICategoryService
    {
        Task<List<CategoryDto>> GetAllAsync();
        Task<CategoryDto?> GetByIdAsync(int id);
        Task<CategoryDto> CreateAsync(CreateCategoryDto dto);
        Task<CategoryDto?> UpdateAsync(int id, UpdateCategoryDto dto);
        Task<CategoryDto?> DeleteAsync(int id);
    }
}
