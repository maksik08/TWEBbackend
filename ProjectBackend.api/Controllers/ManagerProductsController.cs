using Microsoft.AspNetCore.Mvc;
using ProjectBackend.api.Filters;
using ProjectBackend.api.Models.Domain;
using ProjectBackend.api.Models.DTO;
using ProjectBackend.api.Services;

namespace ProjectBackend.api.Controllers
{
    /// <summary>
    /// Manager catalog visibility endpoints.
    /// Access: manager or admin.
    /// </summary>
    [Route("api/manager/products")]
    [ApiController]
    [RoleAccess(UserRole.Manager)]
    public class ManagerProductsController : ControllerBase
    {
        private readonly IManagerService _managerService;

        public ManagerProductsController(IManagerService managerService)
        {
            _managerService = managerService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] ProductListRequestDto request, CancellationToken cancellationToken)
        {
            var products = await _managerService.GetProductsAsync(request, cancellationToken);
            return Ok(PagedResponse<ProductDto>.Ok(products));
        }

        [HttpPatch("{id:int}/visibility")]
        public async Task<IActionResult> UpdateVisibility([FromRoute] int id, [FromBody] UpdateProductVisibilityDto dto, CancellationToken cancellationToken)
        {
            var updated = await _managerService.UpdateProductVisibilityAsync(id, dto, cancellationToken);
            return Ok(ApiResponse<ProductDto>.Ok(updated, "Product visibility updated successfully."));
        }
    }
}
