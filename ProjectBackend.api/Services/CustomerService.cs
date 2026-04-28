using AutoMapper;
using ProjectBackend.api.Exceptions;
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

        public async Task<CustomerDto> GetByIdAsync(int id)
        {
            var customer = await _repository.GetByIdAsync(id);
            if (customer is null)
            {
                throw new NotFoundException($"Customer with id {id} was not found.");
            }

            return _mapper.Map<CustomerDto>(customer);
        }

        public async Task<CustomerDto> CreateAsync(CreateCustomerDto dto)
        {
            var entity = _mapper.Map<CustomerDomain>(dto);
            var created = await _repository.CreateAsync(entity);
            return _mapper.Map<CustomerDto>(created);
        }

        public async Task<CustomerDto> UpdateAsync(int id, UpdateCustomerDto dto)
        {
            var entity = _mapper.Map<CustomerDomain>(dto);
            var updated = await _repository.UpdateAsync(id, entity);
            if (updated is null)
            {
                throw new NotFoundException($"Customer with id {id} was not found.");
            }

            return _mapper.Map<CustomerDto>(updated);
        }

        public async Task<CustomerDto> DeleteAsync(int id)
        {
            var deleted = await _repository.DeleteAsync(id);
            if (deleted is null)
            {
                throw new NotFoundException($"Customer with id {id} was not found.");
            }

            return _mapper.Map<CustomerDto>(deleted);
        }
    }
}
