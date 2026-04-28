using ProjectBackend.api.Models.DTO;

namespace ProjectBackend.api.Services
{
    public interface ISupplierService
    {
        Task<List<SupplierDto>> GetAllAsync();
        Task<SupplierDto> GetByIdAsync(int id);
        Task<SupplierDto> CreateAsync(CreateSupplierDto dto);
        Task<SupplierDto> UpdateAsync(int id, UpdateSupplierDto dto);
        Task<SupplierDto> DeleteAsync(int id);
    }
}
