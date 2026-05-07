using AutoMapper;
using ProjectBackend.api.Exceptions;
using ProjectBackend.api.Models.Common;
using ProjectBackend.api.Models.Domain;
using ProjectBackend.api.Models.DTO;
using ProjectBackend.api.Models.Query;
using ProjectBackend.api.Repositories;

namespace ProjectBackend.api.Services
{
    public class ProductService : ApplicationServiceBase, IProductService
    {
        private readonly IProductRepository _repository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ISupplierRepository _supplierRepository;
        private readonly IImageStorageService _imageStorageService;
        private readonly IMapper _mapper;

        public ProductService(
            IProductRepository repository,
            ICategoryRepository categoryRepository,
            ISupplierRepository supplierRepository,
            IImageStorageService imageStorageService,
            IMapper mapper)
        {
            _repository = repository;
            _categoryRepository = categoryRepository;
            _supplierRepository = supplierRepository;
            _imageStorageService = imageStorageService;
            _mapper = mapper;
        }

        public async Task<PagedResult<ProductDto>> GetAllAsync(ProductListRequestDto request, CancellationToken cancellationToken)
        {
            if (request.MinPrice.HasValue && request.MaxPrice.HasValue && request.MinPrice > request.MaxPrice)
            {
                throw new ValidationException("MinPrice cannot be greater than MaxPrice.");
            }

            var queryOptions = new ProductQueryOptions
            {
                Page = QueryValidationHelper.NormalizePage(request.Page),
                PageSize = QueryValidationHelper.NormalizePageSize(request.PageSize),
                SortBy = QueryValidationHelper.NormalizeSortBy(request.SortBy, "name", "name", "title", "price"),
                SortDescending = QueryValidationHelper.NormalizeSortDescending(request.SortDirection),
                Search = QueryValidationHelper.NormalizeSearch(request.Search),
                CategoryId = request.CategoryId,
                SupplierId = request.SupplierId,
                MinPrice = request.MinPrice,
                MaxPrice = request.MaxPrice
            };

            var products = await _repository.GetAllAsync(queryOptions, cancellationToken);
            return new PagedResult<ProductDto>
            {
                Items = _mapper.Map<IReadOnlyCollection<ProductDto>>(products.Items),
                TotalCount = products.TotalCount,
                Page = products.Page,
                PageSize = products.PageSize
            };
        }

        public async Task<ProductDto> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            var product = EnsureFound(await _repository.GetByIdAsync(id, cancellationToken), "Product", id);
            return _mapper.Map<ProductDto>(product);
        }

        public async Task<ProductDto> CreateAsync(CreateProductDto dto, CancellationToken cancellationToken)
        {
            await ValidateReferencesAsync(dto.CategoryId, dto.SupplierId, cancellationToken);
            var normalizedName = NormalizeRequiredText(dto.Name, "Product name");
            EnsureMinimumValue(dto.Price, 0.01m, "Price");
            var entity = _mapper.Map<ProductsDomain>(dto);
            entity.Name = normalizedName;
            entity.Title = NormalizeOptionalText(dto.Title);
            entity.Image = NormalizeOptionalText(dto.Image);
            var created = await _repository.CreateAsync(entity, cancellationToken);
            return _mapper.Map<ProductDto>(created);
        }

        public async Task<ProductDto> UpdateAsync(int id, UpdateProductDto dto, CancellationToken cancellationToken)
        {
            await ValidateReferencesAsync(dto.CategoryId, dto.SupplierId, cancellationToken);
            var normalizedName = NormalizeRequiredText(dto.Name, "Product name");
            EnsureMinimumValue(dto.Price, 0.01m, "Price");
            var entity = _mapper.Map<ProductsDomain>(dto);
            entity.Name = normalizedName;
            entity.Title = NormalizeOptionalText(dto.Title);
            entity.Image = NormalizeOptionalText(dto.Image);
            var updated = await _repository.UpdateAsync(id, entity, cancellationToken);
            updated = EnsureFound(updated, "Product", id);
            return _mapper.Map<ProductDto>(updated);
        }

        public async Task<ProductDto> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            var deleted = EnsureFound(await _repository.DeleteAsync(id, cancellationToken), "Product", id);
            _imageStorageService.TryDeleteProductImage(deleted.Image);
            return _mapper.Map<ProductDto>(deleted);
        }

        private async Task ValidateReferencesAsync(int? categoryId, int? supplierId, CancellationToken cancellationToken)
        {
            if (categoryId.HasValue && !await _categoryRepository.ExistsAsync(categoryId.Value, cancellationToken))
            {
                throw new ValidationException("The selected category does not exist.");
            }

            if (supplierId.HasValue && !await _supplierRepository.ExistsAsync(supplierId.Value, cancellationToken))
            {
                throw new ValidationException("The selected supplier does not exist.");
            }
        }
    }
}
