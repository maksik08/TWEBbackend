using ProjectBackend.api.Models.Domain;

namespace ProjectBackend.api.Repositories
{
    public interface ISupplierRepository
    {
        Task<List<SupplierDomain>> GetAllAsync();
        Task<SupplierDomain?> GetByIdAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<bool> HasProductsAsync(int id);
        Task<SupplierDomain> CreateAsync(SupplierDomain supplier);
        Task<SupplierDomain?> UpdateAsync(int id, SupplierDomain supplier);
        Task<SupplierDomain?> DeleteAsync(int id);
    }
}
