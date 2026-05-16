using Microsoft.AspNetCore.Mvc;
using ProjectBackend.api.Filters;
using ProjectBackend.api.Models.Domain;
using ProjectBackend.api.Models.DTO;
using ProjectBackend.api.Services;

namespace ProjectBackend.api.Controllers
{
    /// <summary>
    /// Manager service request workflow endpoints.
    /// Access: manager or admin.
    /// </summary>
    [Route("api/manager/requests")]
    [ApiController]
    [RoleAccess(UserRole.Manager)]
    public class ManagerRequestsController : ControllerBase
    {
        private readonly IManagerService _managerService;

        public ManagerRequestsController(IManagerService managerService)
        {
            _managerService = managerService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] ServiceRequestListRequestDto request, CancellationToken cancellationToken)
        {
            var requests = await _managerService.GetRequestsAsync(request, cancellationToken);
            return Ok(PagedResponse<ServiceRequestDto>.Ok(requests));
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken cancellationToken)
        {
            var request = await _managerService.GetRequestByIdAsync(id, cancellationToken);
            return Ok(ApiResponse<ServiceRequestDto>.Ok(request));
        }

        [HttpGet("installers")]
        public async Task<IActionResult> GetInstallers(CancellationToken cancellationToken)
        {
            var installers = await _managerService.GetInstallersAsync(cancellationToken);
            return Ok(ApiResponse<IReadOnlyCollection<InstallerLookupDto>>.Ok(installers));
        }

        [HttpPatch("{id:int}/assign")]
        public async Task<IActionResult> AssignInstaller([FromRoute] int id, [FromBody] AssignInstallerDto dto, CancellationToken cancellationToken)
        {
            var updated = await _managerService.AssignInstallerAsync(id, dto, cancellationToken);
            return Ok(ApiResponse<ServiceRequestDto>.Ok(updated, "Installer assigned successfully."));
        }

        [HttpPost("{id:int}/comments")]
        public async Task<IActionResult> AddComment([FromRoute] int id, [FromBody] AddServiceRequestCommentDto dto, CancellationToken cancellationToken)
        {
            var comment = await _managerService.AddCommentAsync(id, dto, cancellationToken);
            return Ok(ApiResponse<ServiceRequestCommentDto>.Ok(comment, "Comment added successfully."));
        }
    }
}
