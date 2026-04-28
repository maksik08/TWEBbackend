using AutoMapper;
using ProjectBackend.api.Exceptions;
using ProjectBackend.api.Models.Domain;
using ProjectBackend.api.Models.DTO;
using ProjectBackend.api.Repositories;

namespace ProjectBackend.api.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _repository;
        private readonly IMapper _mapper;

        public CategoryService(ICategoryRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<CategoryDto>> GetAllAsync()
        {
            var categories = await _repository.GetAllAsync();
            return _mapper.Map<List<CategoryDto>>(categories);
        }

        public async Task<CategoryDto> GetByIdAsync(int id)
        {
            var category = await _repository.GetByIdAsync(id);
            if (category is null)
            {
                throw new NotFoundException($"Category with id {id} was not found.");
            }

            return _mapper.Map<CategoryDto>(category);
        }

        public async Task<CategoryDto> CreateAsync(CreateCategoryDto dto)
        {
            var entity = _mapper.Map<CategoryDomain>(dto);
            var created = await _repository.CreateAsync(entity);
            return _mapper.Map<CategoryDto>(created);
        }

        public async Task<CategoryDto> UpdateAsync(int id, UpdateCategoryDto dto)
        {
            var entity = _mapper.Map<CategoryDomain>(dto);
            var updated = await _repository.UpdateAsync(id, entity);
            if (updated is null)
            {
                throw new NotFoundException($"Category with id {id} was not found.");
            }

            return _mapper.Map<CategoryDto>(updated);
        }

        public async Task<CategoryDto> DeleteAsync(int id)
        {
            if (await _repository.HasProductsAsync(id))
            {
                throw new ValidationException("The category cannot be deleted because it is used by existing products.");
            }

            var deleted = await _repository.DeleteAsync(id);
            if (deleted is null)
            {
                throw new NotFoundException($"Category with id {id} was not found.");
            }

            return _mapper.Map<CategoryDto>(deleted);
        }
    }
}
