using ProjectBackend.Domain.Common;
using ProjectBackend.BusinessLogic.Dto;

namespace ProjectBackend.BusinessLogic.Services
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
