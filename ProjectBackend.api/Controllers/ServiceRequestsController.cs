using Microsoft.AspNetCore.Mvc;
using ProjectBackend.api.Filters;
using ProjectBackend.Domain.Entities;
using ProjectBackend.BusinessLogic.Dto;
using ProjectBackend.BusinessLogic.Services;

namespace ProjectBackend.api.Controllers
{
    /// <summary>
    /// Customer connection request endpoints.
    /// Access: authenticated customer or admin.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [RoleAccess(UserRole.User, UserRole.Customer)]
    public class ServiceRequestsController : ControllerBase
    {
        private readonly IServiceRequestService _serviceRequestService;

        public ServiceRequestsController(IServiceRequestService serviceRequestService)
        {
            _serviceRequestService = serviceRequestService;
        }

        [HttpGet("my")]
        public async Task<IActionResult> GetMy([FromQuery] ServiceRequestListRequestDto request, CancellationToken cancellationToken)
        {
            var requests = await _serviceRequestService.GetMyRequestsAsync(request, cancellationToken);
            return Ok(PagedResponse<ServiceRequestDto>.Ok(requests));
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken cancellationToken)
        {
            var request = await _serviceRequestService.GetByIdAsync(id, cancellationToken);
            return Ok(ApiResponse<ServiceRequestDto>.Ok(request));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateServiceRequestDto dto, CancellationToken cancellationToken)
        {
            var created = await _serviceRequestService.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, ApiResponse<ServiceRequestDto>.Ok(created, "Service request created successfully."));
        }

        /// <summary>
        /// Pays for the service request from the customer's internal balance.
        /// </summary>
        [HttpPost("{id:int}/pay")]
        public async Task<IActionResult> Pay([FromRoute] int id, CancellationToken cancellationToken)
        {
            var request = await _serviceRequestService.PayAsync(id, cancellationToken);
            return Ok(ApiResponse<ServiceRequestDto>.Ok(request, "Service paid successfully."));
        }

        /// <summary>
        /// Rates the quality of a completed service request (1..5).
        /// </summary>
        [HttpPost("{id:int}/rate")]
        public async Task<IActionResult> Rate([FromRoute] int id, [FromBody] RateServiceRequestDto dto, CancellationToken cancellationToken)
        {
            var request = await _serviceRequestService.RateAsync(id, dto, cancellationToken);
            return Ok(ApiResponse<ServiceRequestDto>.Ok(request, "Thank you for your feedback."));
        }
    }
}
