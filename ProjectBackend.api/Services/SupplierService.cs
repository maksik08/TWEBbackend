using AutoMapper;
using ProjectBackend.api.Exceptions;
using ProjectBackend.api.Models.Domain;
using ProjectBackend.api.Models.DTO;
using ProjectBackend.api.Repositories;

namespace ProjectBackend.api.Services
{
    public class SupplierService : ISupplierService
    {
        private readonly ISupplierRepository _repository;
        private readonly IMapper _mapper;

        public SupplierService(ISupplierRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<SupplierDto>> GetAllAsync()
        {
            var suppliers = await _repository.GetAllAsync();
            return _mapper.Map<List<SupplierDto>>(suppliers);
        }

        public async Task<SupplierDto> GetByIdAsync(int id)
        {
            var supplier = await _repository.GetByIdAsync(id);
            if (supplier is null)
            {
                throw new NotFoundException($"Supplier with id {id} was not found.");
            }

            return _mapper.Map<SupplierDto>(supplier);
        }

        public async Task<SupplierDto> CreateAsync(CreateSupplierDto dto)
        {
            var entity = _mapper.Map<SupplierDomain>(dto);
            var created = await _repository.CreateAsync(entity);
            return _mapper.Map<SupplierDto>(created);
        }

        public async Task<SupplierDto> UpdateAsync(int id, UpdateSupplierDto dto)
        {
            var entity = _mapper.Map<SupplierDomain>(dto);
            var updated = await _repository.UpdateAsync(id, entity);
            if (updated is null)
            {
                throw new NotFoundException($"Supplier with id {id} was not found.");
            }

            return _mapper.Map<SupplierDto>(updated);
        }

        public async Task<SupplierDto> DeleteAsync(int id)
        {
            if (await _repository.HasProductsAsync(id))
            {
                throw new ValidationException("The supplier cannot be deleted because it is used by existing products.");
            }

            var deleted = await _repository.DeleteAsync(id);
            if (deleted is null)
            {
                throw new NotFoundException($"Supplier with id {id} was not found.");
            }

            return _mapper.Map<SupplierDto>(deleted);
        }
    }
}
