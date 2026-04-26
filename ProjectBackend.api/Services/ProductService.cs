using AutoMapper;
using ProjectBackend.api.Models.Domain;
using ProjectBackend.api.Models.DTO;
using ProjectBackend.api.Repositories;

namespace ProjectBackend.api.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ISupplierRepository _supplierRepository;
        private readonly IMapper _mapper;

        public ProductService(
            IProductRepository repository,
            ICategoryRepository categoryRepository,
            ISupplierRepository supplierRepository,
            IMapper mapper)
        {
            _repository = repository;
            _categoryRepository = categoryRepository;
            _supplierRepository = supplierRepository;
            _mapper = mapper;
        }

        public async Task<List<ProductDto>> GetAllAsync()
        {
            var products = await _repository.GetAllAsync();
            return _mapper.Map<List<ProductDto>>(products);
        }

        public async Task<ProductDto?> GetByIdAsync(int id)
        {
            var product = await _repository.GetByIdAsync(id);
            return product is null ? null : _mapper.Map<ProductDto>(product);
        }

        public async Task<ProductDto> CreateAsync(CreateProductDto dto)
        {
            await ValidateReferencesAsync(dto.CategoryId, dto.SupplierId);
            var entity = _mapper.Map<ProductsDomain>(dto);
            var created = await _repository.CreateAsync(entity);
            return _mapper.Map<ProductDto>(created);
        }

        public async Task<ProductDto?> UpdateAsync(int id, UpdateProductDto dto)
        {
            await ValidateReferencesAsync(dto.CategoryId, dto.SupplierId);
            var entity = _mapper.Map<ProductsDomain>(dto);
            var updated = await _repository.UpdateAsync(id, entity);
            return updated is null ? null : _mapper.Map<ProductDto>(updated);
        }

        public async Task<ProductDto?> DeleteAsync(int id)
        {
            var deleted = await _repository.DeleteAsync(id);
            return deleted is null ? null : _mapper.Map<ProductDto>(deleted);
        }

        private async Task ValidateReferencesAsync(int? categoryId, int? supplierId)
        {
            if (categoryId.HasValue && !await _categoryRepository.ExistsAsync(categoryId.Value))
            {
                throw new InvalidOperationException("The selected category does not exist.");
            }

            if (supplierId.HasValue && !await _supplierRepository.ExistsAsync(supplierId.Value))
            {
                throw new InvalidOperationException("The selected supplier does not exist.");
            }
        }
    }
}
