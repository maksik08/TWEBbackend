using AutoMapper;
using ProjectBackend.api.Exceptions;
using ProjectBackend.api.Models.Common;
using ProjectBackend.api.Models.Domain;
using ProjectBackend.api.Models.DTO;
using ProjectBackend.api.Models.Query;
using ProjectBackend.api.Repositories;

namespace ProjectBackend.api.Services
{
    public class CustomerService : ApplicationServiceBase, ICustomerService
    {
        private readonly ICustomerRepository _repository;
        private readonly IMapper _mapper;

        public CustomerService(ICustomerRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<PagedResult<CustomerDto>> GetAllAsync(CustomerListRequestDto request, CancellationToken cancellationToken)
        {
            var queryOptions = new CustomerQueryOptions
            {
                Page = QueryValidationHelper.NormalizePage(request.Page),
                PageSize = QueryValidationHelper.NormalizePageSize(request.PageSize),
                SortBy = QueryValidationHelper.NormalizeSortBy(request.SortBy, "lastname", "firstname", "lastname", "email"),
                SortDescending = QueryValidationHelper.NormalizeSortDescending(request.SortDirection),
                Search = QueryValidationHelper.NormalizeSearch(request.Search)
            };

            var customers = await _repository.GetAllAsync(queryOptions, cancellationToken);
            return new PagedResult<CustomerDto>
            {
                Items = _mapper.Map<IReadOnlyCollection<CustomerDto>>(customers.Items),
                TotalCount = customers.TotalCount,
                Page = customers.Page,
                PageSize = customers.PageSize
            };
        }

        public async Task<CustomerDto> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            var customer = EnsureFound(await _repository.GetByIdAsync(id, cancellationToken), "Customer", id);
            return _mapper.Map<CustomerDto>(customer);
        }

        public async Task<CustomerDto> CreateAsync(CreateCustomerDto dto, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<CustomerDomain>(dto);
            entity.FirstName = NormalizeRequiredText(dto.FirstName, "First name");
            entity.LastName = NormalizeRequiredText(dto.LastName, "Last name");
            entity.Email = string.IsNullOrWhiteSpace(dto.Email) ? null : NormalizeEmail(dto.Email);
            entity.Phone = NormalizeOptionalText(dto.Phone);
            var created = await _repository.CreateAsync(entity, cancellationToken);
            return _mapper.Map<CustomerDto>(created);
        }

        public async Task<CustomerDto> UpdateAsync(int id, UpdateCustomerDto dto, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<CustomerDomain>(dto);
            entity.FirstName = NormalizeRequiredText(dto.FirstName, "First name");
            entity.LastName = NormalizeRequiredText(dto.LastName, "Last name");
            entity.Email = string.IsNullOrWhiteSpace(dto.Email) ? null : NormalizeEmail(dto.Email);
            entity.Phone = NormalizeOptionalText(dto.Phone);
            var updated = await _repository.UpdateAsync(id, entity, cancellationToken);
            updated = EnsureFound(updated, "Customer", id);
            return _mapper.Map<CustomerDto>(updated);
        }

        public async Task<CustomerDto> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            var deleted = EnsureFound(await _repository.DeleteAsync(id, cancellationToken), "Customer", id);
            return _mapper.Map<CustomerDto>(deleted);
        }
    }
}
