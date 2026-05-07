using Microsoft.AspNetCore.Mvc;
using ProjectBackend.api.Filters;
using ProjectBackend.api.Models.DTO;
using ProjectBackend.api.Services;

namespace ProjectBackend.api.Controllers
{
    /// <summary>
    /// Product catalog endpoints.
    /// Read access: guest.
    /// Write access: admin.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private const long MaxImageUploadSize = 5 * 1024 * 1024;

        private readonly IProductService _productService;
        private readonly IImageStorageService _imageStorageService;

        public ProductsController(IProductService productService, IImageStorageService imageStorageService)
        {
            _productService = productService;
            _imageStorageService = imageStorageService;
        }

        /// <summary>
        /// Returns a paged product list with filtering and sorting.
        /// Example query: page=1&amp;pageSize=10&amp;sortBy=price&amp;sortDirection=desc.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] ProductListRequestDto request, CancellationToken cancellationToken)
        {
            var products = await _productService.GetAllAsync(request, cancellationToken);
            return Ok(PagedResponse<ProductDto>.Ok(products));
        }

        /// <summary>
        /// Returns one product by identifier.
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken cancellationToken)
        {
            var product = await _productService.GetByIdAsync(id, cancellationToken);
            return Ok(ApiResponse<ProductDto>.Ok(product));
        }

        [AdminMod]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProductDto dto, CancellationToken cancellationToken)
        {
            var created = await _productService.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, ApiResponse<ProductDto>.Ok(created, "Product created successfully."));
        }

        [AdminMod]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateProductDto dto, CancellationToken cancellationToken)
        {
            var updated = await _productService.UpdateAsync(id, dto, cancellationToken);
            return Ok(ApiResponse<ProductDto>.Ok(updated, "Product updated successfully."));
        }

        [AdminMod]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
        {
            var deleted = await _productService.DeleteAsync(id, cancellationToken);
            return Ok(ApiResponse<ProductDto>.Ok(deleted, "Product deleted successfully."));
        }

        /// <summary>
        /// Uploads a product image and returns its public relative URL.
        /// </summary>
        [AdminMod]
        [HttpPost("upload-image")]
        [RequestSizeLimit(MaxImageUploadSize)]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadImage([FromForm] UploadImageRequestDto request, CancellationToken cancellationToken)
        {
            var url = await _imageStorageService.SaveProductImageAsync(request.File, cancellationToken);
            return Ok(ApiResponse<UploadImageResponseDto>.Ok(
                new UploadImageResponseDto { Url = url },
                "Image uploaded successfully."));
        }
    }
}
