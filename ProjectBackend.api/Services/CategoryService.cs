using AutoMapper;
using ProjectBackend.api.Exceptions;
using ProjectBackend.api.Models.Common;
using ProjectBackend.api.Models.Domain;
using ProjectBackend.api.Models.DTO;
using ProjectBackend.api.Models.Query;
using ProjectBackend.api.Repositories;

namespace ProjectBackend.api.Services
{
    public class CategoryService : ApplicationServiceBase, ICategoryService
    {
        private readonly ICategoryRepository _repository;
        private readonly IMapper _mapper;

        public CategoryService(ICategoryRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<PagedResult<CategoryDto>> GetAllAsync(CategoryListRequestDto request, CancellationToken cancellationToken)
        {
            var queryOptions = new CategoryQueryOptions
            {
                Page = QueryValidationHelper.NormalizePage(request.Page),
                PageSize = QueryValidationHelper.NormalizePageSize(request.PageSize),
                SortBy = QueryValidationHelper.NormalizeSortBy(request.SortBy, "name", "name", "description"),
                SortDescending = QueryValidationHelper.NormalizeSortDescending(request.SortDirection),
                Search = QueryValidationHelper.NormalizeSearch(request.Search)
            };

            var categories = await _repository.GetAllAsync(queryOptions, cancellationToken);
            return new PagedResult<CategoryDto>
            {
                Items = _mapper.Map<IReadOnlyCollection<CategoryDto>>(categories.Items),
                TotalCount = categories.TotalCount,
                Page = categories.Page,
                PageSize = categories.PageSize
            };
        }

        public async Task<CategoryDto> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            var category = EnsureFound(await _repository.GetByIdAsync(id, cancellationToken), "Category", id);
            return _mapper.Map<CategoryDto>(category);
        }

        public async Task<CategoryDto> CreateAsync(CreateCategoryDto dto, CancellationToken cancellationToken)
        {
            var normalizedName = NormalizeRequiredText(dto.Name, "Category name");
            var description = NormalizeOptionalText(dto.Description);

            if (await _repository.ExistsByNameAsync(normalizedName, null, cancellationToken))
            {
                throw new ConflictException("A category with this name already exists.");
            }

            var entity = _mapper.Map<CategoryDomain>(dto);
            entity.Name = normalizedName;
            entity.Description = description;
            var created = await _repository.CreateAsync(entity, cancellationToken);
            return _mapper.Map<CategoryDto>(created);
        }

        public async Task<CategoryDto> UpdateAsync(int id, UpdateCategoryDto dto, CancellationToken cancellationToken)
        {
            var normalizedName = NormalizeRequiredText(dto.Name, "Category name");
            var description = NormalizeOptionalText(dto.Description);

            if (await _repository.ExistsByNameAsync(normalizedName, id, cancellationToken))
            {
                throw new ConflictException("A category with this name already exists.");
            }

            var entity = _mapper.Map<CategoryDomain>(dto);
            entity.Name = normalizedName;
            entity.Description = description;
            var updated = await _repository.UpdateAsync(id, entity, cancellationToken);
            updated = EnsureFound(updated, "Category", id);
            return _mapper.Map<CategoryDto>(updated);
        }

        public async Task<CategoryDto> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            if (await _repository.HasProductsAsync(id, cancellationToken))
            {
                throw new ValidationException("The category cannot be deleted because it is used by existing products.");
            }

            var deleted = EnsureFound(await _repository.DeleteAsync(id, cancellationToken), "Category", id);
            return _mapper.Map<CategoryDto>(deleted);
        }
    }
}
