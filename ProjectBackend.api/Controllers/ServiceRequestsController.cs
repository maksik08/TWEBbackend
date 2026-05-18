using Microsoft.AspNetCore.Mvc;
using ProjectBackend.api.Filters;
using ProjectBackend.api.Models.Domain;
using ProjectBackend.api.Models.DTO;
using ProjectBackend.api.Services;

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
    }
}
