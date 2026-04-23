using AutoMapper;
using ProjectBackend.api.Models.Domain;
using ProjectBackend.api.Models.DTO;
using ProjectBackend.api.Repositories;

namespace ProjectBackend.api.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _repository;
        private readonly IMapper _mapper;

        public CustomerService(ICustomerRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<CustomerDto>> GetAllAsync()
        {
            var customers = await _repository.GetAllAsync();
            return _mapper.Map<List<CustomerDto>>(customers);
        }

        public async Task<CustomerDto?> GetByIdAsync(int id)
        {
            var customer = await _repository.GetByIdAsync(id);
            return customer is null ? null : _mapper.Map<CustomerDto>(customer);
        }

        public async Task<CustomerDto> CreateAsync(CreateCustomerDto dto)
        {
            var entity = _mapper.Map<CustomerDomain>(dto);
            var created = await _repository.CreateAsync(entity);
            return _mapper.Map<CustomerDto>(created);
        }

        public async Task<CustomerDto?> UpdateAsync(int id, UpdateCustomerDto dto)
        {
            var entity = _mapper.Map<CustomerDomain>(dto);
            var updated = await _repository.UpdateAsync(id, entity);
            return updated is null ? null : _mapper.Map<CustomerDto>(updated);
        }

        public async Task<CustomerDto?> DeleteAsync(int id)
        {
            var deleted = await _repository.DeleteAsync(id);
            return deleted is null ? null : _mapper.Map<CustomerDto>(deleted);
        }
    }
}
