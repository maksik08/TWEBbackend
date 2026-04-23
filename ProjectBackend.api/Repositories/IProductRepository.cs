using ProjectBackend.api.Models.Domain;

namespace ProjectBackend.api.Repositories
{
    public interface IProductRepository
    {
        Task<List<ProductsDomain>> GetAllAsync();
        Task<ProductsDomain?> GetByIdAsync(int id);
        Task<ProductsDomain> CreateAsync(ProductsDomain product);
        Task<ProductsDomain?> UpdateAsync(int id, ProductsDomain product);
        Task<ProductsDomain?> DeleteAsync(int id);
    }
}
