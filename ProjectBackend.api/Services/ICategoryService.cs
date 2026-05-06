using ProjectBackend.api.Models.Common;
using ProjectBackend.api.Models.DTO;

namespace ProjectBackend.api.Services
{
    public interface ICategoryService
    {
        Task<PagedResult<CategoryDto>> GetAllAsync(CategoryListRequestDto request, CancellationToken cancellationToken);
        Task<CategoryDto> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<CategoryDto> CreateAsync(CreateCategoryDto dto, CancellationToken cancellationToken);
        Task<CategoryDto> UpdateAsync(int id, UpdateCategoryDto dto, CancellationToken cancellationToken);
        Task<CategoryDto> DeleteAsync(int id, CancellationToken cancellationToken);
    }
}
