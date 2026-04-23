using AutoMapper;
using ProjectBackend.api.Models.Domain;
using ProjectBackend.api.Models.DTO;
using ProjectBackend.api.Repositories;

namespace ProjectBackend.api.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repository;
        private readonly IMapper _mapper;

        public ProductService(IProductRepository repository, IMapper mapper)
        {
            _repository = repository;
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
            var entity = _mapper.Map<ProductsDomain>(dto);
            var created = await _repository.CreateAsync(entity);
            return _mapper.Map<ProductDto>(created);
        }

        public async Task<ProductDto?> UpdateAsync(int id, UpdateProductDto dto)
        {
            var entity = _mapper.Map<ProductsDomain>(dto);
            var updated = await _repository.UpdateAsync(id, entity);
            return updated is null ? null : _mapper.Map<ProductDto>(updated);
        }

        public async Task<ProductDto?> DeleteAsync(int id)
        {
            var deleted = await _repository.DeleteAsync(id);
            return deleted is null ? null : _mapper.Map<ProductDto>(deleted);
        }
    }
}
