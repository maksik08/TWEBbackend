using ProjectBackend.api.Models.Common;
using ProjectBackend.api.Models.DTO;

namespace ProjectBackend.api.Services
{
    public interface ISupplierService
    {
        Task<PagedResult<SupplierDto>> GetAllAsync(SupplierListRequestDto request, CancellationToken cancellationToken);
        Task<SupplierDto> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<SupplierDto> CreateAsync(CreateSupplierDto dto, CancellationToken cancellationToken);
        Task<SupplierDto> UpdateAsync(int id, UpdateSupplierDto dto, CancellationToken cancellationToken);
        Task<SupplierDto> DeleteAsync(int id, CancellationToken cancellationToken);
    }
}
