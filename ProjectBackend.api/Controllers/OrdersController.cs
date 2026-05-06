using Microsoft.AspNetCore.Mvc;
using ProjectBackend.api.Filters;
using ProjectBackend.api.Models.DTO;
using ProjectBackend.api.Services;

namespace ProjectBackend.api.Controllers
{
    /// <summary>
    /// Customer orders endpoints.
    /// Read/write own orders: any authenticated user.
    /// List all and force status changes: admin.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        /// <summary>
        /// Returns the current user's orders.
        /// </summary>
        [UserAccess]
        [HttpGet("mine")]
        public async Task<IActionResult> GetMine([FromQuery] OrderListRequestDto request, CancellationToken cancellationToken)
        {
            var orders = await _orderService.GetMineAsync(request, cancellationToken);
            return Ok(PagedResponse<OrderDto>.Ok(orders));
        }

        /// <summary>
        /// Returns one order by id. Owner or admin only.
        /// </summary>
        [UserAccess]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken cancellationToken)
        {
            var order = await _orderService.GetByIdAsync(id, cancellationToken);
            return Ok(ApiResponse<OrderDto>.Ok(order));
        }

        /// <summary>
        /// Creates a new pending order from current cart contents.
        /// </summary>
        [UserAccess]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateOrderDto dto, CancellationToken cancellationToken)
        {
            var created = await _orderService.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, ApiResponse<OrderDto>.Ok(created, "Order created successfully."));
        }

        /// <summary>
        /// Marks the pending order as paid (mock payment).
        /// </summary>
        [UserAccess]
        [HttpPost("{id:int}/pay")]
        public async Task<IActionResult> Pay([FromRoute] int id, CancellationToken cancellationToken)
        {
            var paid = await _orderService.PayAsync(id, cancellationToken);
            return Ok(ApiResponse<OrderDto>.Ok(paid, "Order paid successfully."));
        }

        /// <summary>
        /// Cancels a pending or paid order.
        /// </summary>
        [UserAccess]
        [HttpPost("{id:int}/cancel")]
        public async Task<IActionResult> Cancel([FromRoute] int id, CancellationToken cancellationToken)
        {
            var cancelled = await _orderService.CancelAsync(id, cancellationToken);
            return Ok(ApiResponse<OrderDto>.Ok(cancelled, "Order cancelled successfully."));
        }

        /// <summary>
        /// Returns all orders. Admin only.
        /// </summary>
        [AdminMod]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] OrderListRequestDto request, CancellationToken cancellationToken)
        {
            var orders = await _orderService.GetAllAsync(request, cancellationToken);
            return Ok(PagedResponse<OrderDto>.Ok(orders));
        }
    }
}
