using AutoMapper;
using ProjectBackend.api.Exceptions;
using ProjectBackend.api.Models.Common;
using ProjectBackend.api.Models.Domain;
using ProjectBackend.api.Models.DTO;
using ProjectBackend.api.Models.Query;
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

        public async Task<PagedResult<SupplierDto>> GetAllAsync(SupplierListRequestDto request, CancellationToken cancellationToken)
        {
            var queryOptions = new SupplierQueryOptions
            {
                Page = QueryValidationHelper.NormalizePage(request.Page),
                PageSize = QueryValidationHelper.NormalizePageSize(request.PageSize),
                SortBy = QueryValidationHelper.NormalizeSortBy(request.SortBy, "name", "name", "contactemail"),
                SortDescending = QueryValidationHelper.NormalizeSortDescending(request.SortDirection),
                Search = QueryValidationHelper.NormalizeSearch(request.Search)
            };

            var suppliers = await _repository.GetAllAsync(queryOptions, cancellationToken);
            return new PagedResult<SupplierDto>
            {
                Items = _mapper.Map<IReadOnlyCollection<SupplierDto>>(suppliers.Items),
                TotalCount = suppliers.TotalCount,
                Page = suppliers.Page,
                PageSize = suppliers.PageSize
            };
        }

        public async Task<SupplierDto> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            var supplier = await _repository.GetByIdAsync(id, cancellationToken);
            if (supplier is null)
            {
                throw new NotFoundException($"Supplier with id {id} was not found.");
            }

            return _mapper.Map<SupplierDto>(supplier);
        }

        public async Task<SupplierDto> CreateAsync(CreateSupplierDto dto, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<SupplierDomain>(dto);
            var created = await _repository.CreateAsync(entity, cancellationToken);
            return _mapper.Map<SupplierDto>(created);
        }

        public async Task<SupplierDto> UpdateAsync(int id, UpdateSupplierDto dto, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<SupplierDomain>(dto);
            var updated = await _repository.UpdateAsync(id, entity, cancellationToken);
            if (updated is null)
            {
                throw new NotFoundException($"Supplier with id {id} was not found.");
            }

            return _mapper.Map<SupplierDto>(updated);
        }

        public async Task<SupplierDto> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            if (await _repository.HasProductsAsync(id, cancellationToken))
            {
                throw new ValidationException("The supplier cannot be deleted because it is used by existing products.");
            }

            var deleted = await _repository.DeleteAsync(id, cancellationToken);
            if (deleted is null)
            {
                throw new NotFoundException($"Supplier with id {id} was not found.");
            }

            return _mapper.Map<SupplierDto>(deleted);
        }
    }
}
