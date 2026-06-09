using AutoMapper;
using ProjectBackend.Domain.Entities;
using ProjectBackend.BusinessLogic.Dto;
using ProjectBackend.DataAccess.Repositories;

namespace ProjectBackend.BusinessLogic.Services
{
    public class ServiceTariffService : ApplicationServiceBase, IServiceTariffService
    {
        private readonly IServiceTariffRepository _repository;
        private readonly IMapper _mapper;

        public ServiceTariffService(IServiceTariffRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IReadOnlyCollection<ServiceTariffDto>> GetAllAsync(bool activeOnly, CancellationToken cancellationToken)
        {
            var tariffs = await _repository.GetAllAsync(activeOnly, cancellationToken);
            return _mapper.Map<IReadOnlyCollection<ServiceTariffDto>>(tariffs);
        }

        public async Task<ServiceTariffDto> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            var tariff = EnsureFound(await _repository.GetByIdAsync(id, cancellationToken), "Service tariff", id);
            return _mapper.Map<ServiceTariffDto>(tariff);
        }

        public async Task<ServiceTariffDto> CreateAsync(CreateServiceTariffDto dto, CancellationToken cancellationToken)
        {
            var entity = new ServiceTariffDomain
            {
                Name = NormalizeRequiredText(dto.Name, "Name"),
                Description = NormalizeOptionalText(dto.Description),
                Price = dto.Price,
                IsActive = dto.IsActive
            };

            var created = await _repository.CreateAsync(entity, cancellationToken);
            return _mapper.Map<ServiceTariffDto>(created);
        }

        public async Task<ServiceTariffDto> UpdateAsync(int id, UpdateServiceTariffDto dto, CancellationToken cancellationToken)
        {
            EnsureFound(await _repository.GetByIdAsync(id, cancellationToken), "Service tariff", id);

            var entity = new ServiceTariffDomain
            {
                Id = id,
                Name = NormalizeRequiredText(dto.Name, "Name"),
                Description = NormalizeOptionalText(dto.Description),
                Price = dto.Price,
                IsActive = dto.IsActive
            };

            var updated = EnsureFound(await _repository.UpdateAsync(entity, cancellationToken), "Service tariff", id);
            return _mapper.Map<ServiceTariffDto>(updated);
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken)
        {
            EnsureFound(await _repository.DeleteAsync(id, cancellationToken), "Service tariff", id);
        }
    }
}
