using ProjectBackend.Domain.Common;
using ProjectBackend.BusinessLogic.Dto;

namespace ProjectBackend.BusinessLogic.Services
{
    public interface ICustomerService
    {
        Task<PagedResult<CustomerDto>> GetAllAsync(CustomerListRequestDto request, CancellationToken cancellationToken);
        Task<CustomerDto> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<CustomerDto> CreateAsync(CreateCustomerDto dto, CancellationToken cancellationToken);
        Task<CustomerDto> UpdateAsync(int id, UpdateCustomerDto dto, CancellationToken cancellationToken);
        Task<CustomerDto> DeleteAsync(int id, CancellationToken cancellationToken);
    }
}
