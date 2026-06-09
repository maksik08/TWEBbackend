using ProjectBackend.BusinessLogic.Dto;

namespace ProjectBackend.BusinessLogic.Services
{
    public interface IServiceTariffService
    {
        Task<IReadOnlyCollection<ServiceTariffDto>> GetAllAsync(bool activeOnly, CancellationToken cancellationToken);
        Task<ServiceTariffDto> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<ServiceTariffDto> CreateAsync(CreateServiceTariffDto dto, CancellationToken cancellationToken);
        Task<ServiceTariffDto> UpdateAsync(int id, UpdateServiceTariffDto dto, CancellationToken cancellationToken);
        Task DeleteAsync(int id, CancellationToken cancellationToken);
    }
}
