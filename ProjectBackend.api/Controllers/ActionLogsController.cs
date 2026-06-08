using Microsoft.AspNetCore.Mvc;
using ProjectBackend.api.Filters;
using ProjectBackend.BusinessLogic.Dto;
using ProjectBackend.BusinessLogic.Services;

namespace ProjectBackend.api.Controllers
{
    /// <summary>
    /// Audit log of user actions. Access: admin only.
    /// </summary>
    [Route("api/admin/action-logs")]
    [ApiController]
    [AdminMod]
    public class ActionLogsController : ControllerBase
    {
        private readonly IActionLogService _actionLogService;

        public ActionLogsController(IActionLogService actionLogService)
        {
            _actionLogService = actionLogService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] ActionLogListRequestDto request, CancellationToken cancellationToken)
        {
            var logs = await _actionLogService.GetAllAsync(request, cancellationToken);
            return Ok(PagedResponse<ActionLogDto>.Ok(logs));
        }
    }
}
