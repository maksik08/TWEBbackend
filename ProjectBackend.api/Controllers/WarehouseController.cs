using Microsoft.AspNetCore.Mvc;
using ProjectBackend.api.Filters;
using ProjectBackend.BusinessLogic.Dto;
using ProjectBackend.BusinessLogic.Services;

namespace ProjectBackend.api.Controllers
{
    /// <summary>
    /// Warehouse stock integration. Access: admin only.
    /// Pulls stock levels from the (mock) external warehouse and reconciles product stock.
    /// </summary>
    [Route("api/admin/warehouse")]
    [ApiController]
    [AdminMod]
    public class WarehouseController : ControllerBase
    {
        private readonly IWarehouseSyncService _warehouseSyncService;

        public WarehouseController(IWarehouseSyncService warehouseSyncService)
        {
            _warehouseSyncService = warehouseSyncService;
        }

        /// <summary>
        /// Computes the stock changes the warehouse would apply, without persisting them.
        /// </summary>
        [HttpGet("preview")]
        public async Task<IActionResult> Preview(CancellationToken cancellationToken)
        {
            var result = await _warehouseSyncService.SyncAsync(dryRun: true, cancellationToken);
            return Ok(ApiResponse<WarehouseSyncResultDto>.Ok(result));
        }

        /// <summary>
        /// Pulls stock levels from the warehouse and updates product stock.
        /// </summary>
        [HttpPost("sync")]
        public async Task<IActionResult> Sync(CancellationToken cancellationToken)
        {
            var result = await _warehouseSyncService.SyncAsync(dryRun: false, cancellationToken);
            return Ok(ApiResponse<WarehouseSyncResultDto>.Ok(result, "Warehouse stock synchronized."));
        }
    }
}
