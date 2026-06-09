using Microsoft.EntityFrameworkCore;
using ProjectBackend.DataAccess;
using ProjectBackend.Domain.Entities;

namespace ProjectBackend.DataAccess.Repositories
{
    public class ServiceTariffRepository : IServiceTariffRepository
    {
        private readonly ProjectDbContext _dbContext;

        public ServiceTariffRepository(ProjectDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IReadOnlyCollection<ServiceTariffDomain>> GetAllAsync(bool activeOnly, CancellationToken cancellationToken)
        {
            var query = _dbContext.ServiceTariffs.AsNoTracking().AsQueryable();
            if (activeOnly)
            {
                query = query.Where(tariff => tariff.IsActive);
            }

            return await query
                .OrderBy(tariff => tariff.Price)
                .ThenBy(tariff => tariff.Name)
                .ToListAsync(cancellationToken);
        }

        public async Task<ServiceTariffDomain?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await _dbContext.ServiceTariffs.AsNoTracking().FirstOrDefaultAsync(tariff => tariff.Id == id, cancellationToken);
        }

        public async Task<ServiceTariffDomain> CreateAsync(ServiceTariffDomain entity, CancellationToken cancellationToken)
        {
            await _dbContext.ServiceTariffs.AddAsync(entity, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return entity;
        }

        public async Task<ServiceTariffDomain?> UpdateAsync(ServiceTariffDomain entity, CancellationToken cancellationToken)
        {
            var existing = await _dbContext.ServiceTariffs.FirstOrDefaultAsync(tariff => tariff.Id == entity.Id, cancellationToken);
            if (existing is null)
            {
                return null;
            }

            existing.Name = entity.Name;
            existing.Description = entity.Description;
            existing.Price = entity.Price;
            existing.IsActive = entity.IsActive;
            await _dbContext.SaveChangesAsync(cancellationToken);
            return existing;
        }

        public async Task<ServiceTariffDomain?> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            var existing = await _dbContext.ServiceTariffs.FirstOrDefaultAsync(tariff => tariff.Id == id, cancellationToken);
            if (existing is null)
            {
                return null;
            }

            _dbContext.ServiceTariffs.Remove(existing);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return existing;
        }
    }
}
