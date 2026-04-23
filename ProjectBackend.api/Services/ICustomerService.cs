using ProjectBackend.api.Models.DTO;

namespace ProjectBackend.api.Services
{
    public interface ICustomerService
    {
        Task<List<CustomerDto>> GetAllAsync();
        Task<CustomerDto?> GetByIdAsync(int id);
        Task<CustomerDto> CreateAsync(CreateCustomerDto dto);
        Task<CustomerDto?> UpdateAsync(int id, UpdateCustomerDto dto);
        Task<CustomerDto?> DeleteAsync(int id);
    }
}
