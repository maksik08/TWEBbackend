using ProjectBackend.Domain.Entities;

namespace ProjectBackend.DataAccess.Repositories
{
    public interface IServiceTariffRepository
    {
        Task<IReadOnlyCollection<ServiceTariffDomain>> GetAllAsync(bool activeOnly, CancellationToken cancellationToken);
        Task<ServiceTariffDomain?> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<ServiceTariffDomain> CreateAsync(ServiceTariffDomain entity, CancellationToken cancellationToken);
        Task<ServiceTariffDomain?> UpdateAsync(ServiceTariffDomain entity, CancellationToken cancellationToken);
        Task<ServiceTariffDomain?> DeleteAsync(int id, CancellationToken cancellationToken);
    }
}
