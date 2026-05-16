using Microsoft.AspNetCore.Mvc;
using ProjectBackend.api.Filters;
using ProjectBackend.api.Models.Domain;
using ProjectBackend.api.Models.DTO;
using ProjectBackend.api.Services;

namespace ProjectBackend.api.Controllers
{
    /// <summary>
    /// Manager order workflow endpoints.
    /// Access: manager or admin.
    /// </summary>
    [Route("api/manager/orders")]
    [ApiController]
    [RoleAccess(UserRole.Manager)]
    public class ManagerOrdersController : ControllerBase
    {
        private readonly IManagerService _managerService;

        public ManagerOrdersController(IManagerService managerService)
        {
            _managerService = managerService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] OrderListRequestDto request, CancellationToken cancellationToken)
        {
            var orders = await _managerService.GetOrdersAsync(request, cancellationToken);
            return Ok(PagedResponse<OrderDto>.Ok(orders));
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken cancellationToken)
        {
            var order = await _managerService.GetOrderByIdAsync(id, cancellationToken);
            return Ok(ApiResponse<OrderDto>.Ok(order));
        }

        [HttpPatch("{id:int}/status")]
        public async Task<IActionResult> UpdateStatus([FromRoute] int id, [FromBody] UpdateOrderStatusDto dto, CancellationToken cancellationToken)
        {
            var updated = await _managerService.UpdateOrderStatusAsync(id, dto, cancellationToken);
            return Ok(ApiResponse<OrderDto>.Ok(updated, "Order status updated successfully."));
        }
    }
}
