using ProjectBackend.api.Models.DTO;

namespace ProjectBackend.api.Services
{
    public interface IProductService
    {
        Task<List<ProductDto>> GetAllAsync();
        Task<ProductDto> GetByIdAsync(int id);
        Task<ProductDto> CreateAsync(CreateProductDto dto);
        Task<ProductDto> UpdateAsync(int id, UpdateProductDto dto);
        Task<ProductDto> DeleteAsync(int id);
    }
}
