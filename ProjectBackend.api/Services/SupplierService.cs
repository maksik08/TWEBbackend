using AutoMapper;
using ProjectBackend.api.Exceptions;
using ProjectBackend.api.Models.Common;
using ProjectBackend.api.Models.Domain;
using ProjectBackend.api.Models.DTO;
using ProjectBackend.api.Models.Query;
using ProjectBackend.api.Repositories;

namespace ProjectBackend.api.Services
{
    public class SupplierService : ApplicationServiceBase, ISupplierService
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
            var supplier = EnsureFound(await _repository.GetByIdAsync(id, cancellationToken), "Supplier", id);
            return _mapper.Map<SupplierDto>(supplier);
        }

        public async Task<SupplierDto> CreateAsync(CreateSupplierDto dto, CancellationToken cancellationToken)
        {
            var normalizedName = NormalizeRequiredText(dto.Name, "Supplier name");
            var normalizedEmail = string.IsNullOrWhiteSpace(dto.ContactEmail) ? null : NormalizeEmail(dto.ContactEmail, "Supplier email");

            if (await _repository.ExistsByNameAsync(normalizedName, null, cancellationToken))
            {
                throw new ConflictException("A supplier with this name already exists.");
            }

            if (normalizedEmail is not null &&
                await _repository.ExistsByContactEmailAsync(normalizedEmail, null, cancellationToken))
            {
                throw new ConflictException("A supplier with this contact email already exists.");
            }

            var entity = _mapper.Map<SupplierDomain>(dto);
            entity.Name = normalizedName;
            entity.ContactEmail = normalizedEmail;
            entity.Phone = NormalizeOptionalText(dto.Phone);
            var created = await _repository.CreateAsync(entity, cancellationToken);
            return _mapper.Map<SupplierDto>(created);
        }

        public async Task<SupplierDto> UpdateAsync(int id, UpdateSupplierDto dto, CancellationToken cancellationToken)
        {
            var normalizedName = NormalizeRequiredText(dto.Name, "Supplier name");
            var normalizedEmail = string.IsNullOrWhiteSpace(dto.ContactEmail) ? null : NormalizeEmail(dto.ContactEmail, "Supplier email");

            if (await _repository.ExistsByNameAsync(normalizedName, id, cancellationToken))
            {
                throw new ConflictException("A supplier with this name already exists.");
            }

            if (normalizedEmail is not null &&
                await _repository.ExistsByContactEmailAsync(normalizedEmail, id, cancellationToken))
            {
                throw new ConflictException("A supplier with this contact email already exists.");
            }

            var entity = _mapper.Map<SupplierDomain>(dto);
            entity.Name = normalizedName;
            entity.ContactEmail = normalizedEmail;
            entity.Phone = NormalizeOptionalText(dto.Phone);
            var updated = await _repository.UpdateAsync(id, entity, cancellationToken);
            updated = EnsureFound(updated, "Supplier", id);
            return _mapper.Map<SupplierDto>(updated);
        }

        public async Task<SupplierDto> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            if (await _repository.HasProductsAsync(id, cancellationToken))
            {
                throw new ValidationException("The supplier cannot be deleted because it is used by existing products.");
            }

            var deleted = EnsureFound(await _repository.DeleteAsync(id, cancellationToken), "Supplier", id);
            return _mapper.Map<SupplierDto>(deleted);
        }
    }
}
