using Microsoft.AspNetCore.Mvc;
using ProjectBackend.api.Filters;
using ProjectBackend.api.Models.DTO;
using ProjectBackend.api.Services;

namespace ProjectBackend.api.Controllers
{
    /// <summary>
    /// Category endpoints.
    /// Read access: authenticated user or admin.
    /// Write access: admin.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [UserAccess]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        /// <summary>
        /// Returns a paged category list with filtering and sorting.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] CategoryListRequestDto request, CancellationToken cancellationToken)
        {
            var categories = await _categoryService.GetAllAsync(request, cancellationToken);
            return Ok(PagedResponse<CategoryDto>.Ok(categories));
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken cancellationToken)
        {
            var category = await _categoryService.GetByIdAsync(id, cancellationToken);
            return Ok(ApiResponse<CategoryDto>.Ok(category));
        }

        [AdminMod]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCategoryDto dto, CancellationToken cancellationToken)
        {
            var created = await _categoryService.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, ApiResponse<CategoryDto>.Ok(created, "Category created successfully."));
        }

        [AdminMod]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateCategoryDto dto, CancellationToken cancellationToken)
        {
            var updated = await _categoryService.UpdateAsync(id, dto, cancellationToken);
            return Ok(ApiResponse<CategoryDto>.Ok(updated, "Category updated successfully."));
        }

        [AdminMod]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
        {
            var deleted = await _categoryService.DeleteAsync(id, cancellationToken);
            return Ok(ApiResponse<CategoryDto>.Ok(deleted, "Category deleted successfully."));
        }
    }
}
