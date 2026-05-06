using ProjectBackend.api.Models.Common;
using ProjectBackend.api.Models.DTO;

namespace ProjectBackend.api.Services
{
    public interface IProductService
    {
        Task<PagedResult<ProductDto>> GetAllAsync(ProductListRequestDto request, CancellationToken cancellationToken);
        Task<ProductDto> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<ProductDto> CreateAsync(CreateProductDto dto, CancellationToken cancellationToken);
        Task<ProductDto> UpdateAsync(int id, UpdateProductDto dto, CancellationToken cancellationToken);
        Task<ProductDto> DeleteAsync(int id, CancellationToken cancellationToken);
    }
}
