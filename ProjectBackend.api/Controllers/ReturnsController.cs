using Microsoft.AspNetCore.Mvc;
using ProjectBackend.api.Filters;
using ProjectBackend.BusinessLogic.Dto;
using ProjectBackend.BusinessLogic.Services;

namespace ProjectBackend.api.Controllers
{
    /// <summary>
    /// Returns (RMA) management. Access: admin only.
    /// Admins initiate returns for paid/shipped/completed orders and approve or reject them.
    /// Approving refunds the order total to the customer's balance and restocks the items.
    /// </summary>
    [Route("api/admin/returns")]
    [ApiController]
    [AdminMod]
    public class ReturnsController : ControllerBase
    {
        private readonly IReturnService _returnService;

        public ReturnsController(IReturnService returnService)
        {
            _returnService = returnService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] ReturnListRequestDto request, CancellationToken cancellationToken)
        {
            var returns = await _returnService.GetAllAsync(request, cancellationToken);
            return Ok(PagedResponse<ReturnDto>.Ok(returns));
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken cancellationToken)
        {
            var entity = await _returnService.GetByIdAsync(id, cancellationToken);
            return Ok(ApiResponse<ReturnDto>.Ok(entity));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateReturnDto dto, CancellationToken cancellationToken)
        {
            var created = await _returnService.CreateAsync(dto, cancellationToken);
            return Ok(ApiResponse<ReturnDto>.Ok(created, "Return initiated."));
        }

        [HttpPost("{id:int}/approve")]
        public async Task<IActionResult> Approve([FromRoute] int id, [FromBody] ResolveReturnDto dto, CancellationToken cancellationToken)
        {
            var entity = await _returnService.ApproveAsync(id, dto, cancellationToken);
            return Ok(ApiResponse<ReturnDto>.Ok(entity, "Return approved and refunded."));
        }

        [HttpPost("{id:int}/reject")]
        public async Task<IActionResult> Reject([FromRoute] int id, [FromBody] ResolveReturnDto dto, CancellationToken cancellationToken)
        {
            var entity = await _returnService.RejectAsync(id, dto, cancellationToken);
            return Ok(ApiResponse<ReturnDto>.Ok(entity, "Return rejected."));
        }
    }
}
