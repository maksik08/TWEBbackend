using Microsoft.AspNetCore.Mvc;
using ProjectBackend.api.Filters;
using ProjectBackend.api.Models.Domain;
using ProjectBackend.api.Models.DTO;
using ProjectBackend.api.Services;

namespace ProjectBackend.api.Controllers
{
    /// <summary>
    /// Installer workflow endpoints.
    /// Access: installer or admin.
    /// </summary>
    [Route("api/installer")]
    [ApiController]
    [RoleAccess(UserRole.Installer)]
    public class InstallerRequestsController : ControllerBase
    {
        private readonly IInstallerService _installerService;

        public InstallerRequestsController(IInstallerService installerService)
        {
            _installerService = installerService;
        }

        [HttpGet("my-requests")]
        public async Task<IActionResult> GetMyRequests([FromQuery] ServiceRequestListRequestDto request, CancellationToken cancellationToken)
        {
            var requests = await _installerService.GetMyRequestsAsync(request, cancellationToken);
            return Ok(PagedResponse<ServiceRequestDto>.Ok(requests));
        }

        [HttpGet("route")]
        public async Task<IActionResult> GetRoute([FromQuery] DateTime? date, CancellationToken cancellationToken)
        {
            var route = await _installerService.GetRouteAsync(date, cancellationToken);
            return Ok(ApiResponse<IReadOnlyCollection<ServiceRequestDto>>.Ok(route));
        }

        [HttpPost("requests/{id:int}/start")]
        public async Task<IActionResult> Start([FromRoute] int id, CancellationToken cancellationToken)
        {
            var updated = await _installerService.StartRequestAsync(id, cancellationToken);
            return Ok(ApiResponse<ServiceRequestDto>.Ok(updated, "Request started successfully."));
        }

        [HttpPost("requests/{id:int}/complete")]
        public async Task<IActionResult> Complete([FromRoute] int id, [FromForm] CompleteServiceRequestDto dto, CancellationToken cancellationToken)
        {
            var updated = await _installerService.CompleteRequestAsync(id, dto, cancellationToken);
            return Ok(ApiResponse<ServiceRequestDto>.Ok(updated, "Request completed successfully."));
        }
    }
}
