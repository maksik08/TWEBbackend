using Microsoft.AspNetCore.Mvc;
using ProjectBackend.api.Filters;
using ProjectBackend.Domain.Entities;
using ProjectBackend.BusinessLogic.Dto;
using ProjectBackend.BusinessLogic.Services;

namespace ProjectBackend.api.Controllers
{
    /// <summary>
    /// Manager reporting endpoints.
    /// Access: manager or admin.
    /// </summary>
    [Route("api/manager/reports")]
    [ApiController]
    [RoleAccess(UserRole.Manager)]
    public class ManagerReportsController : ControllerBase
    {
        private readonly IManagerService _managerService;

        public ManagerReportsController(IManagerService managerService)
        {
            _managerService = managerService;
        }

        [HttpGet("sales")]
        public async Task<IActionResult> GetSales(CancellationToken cancellationToken)
        {
            var report = await _managerService.GetSalesReportAsync(cancellationToken);
            return Ok(ApiResponse<SalesReportDto>.Ok(report));
        }

        [HttpGet("requests")]
        public async Task<IActionResult> GetRequests(CancellationToken cancellationToken)
        {
            var report = await _managerService.GetServiceRequestReportAsync(cancellationToken);
            return Ok(ApiResponse<ServiceRequestReportDto>.Ok(report));
        }

        [HttpGet("stock")]
        public async Task<IActionResult> GetStock(CancellationToken cancellationToken)
        {
            var report = await _managerService.GetStockReportAsync(cancellationToken);
            return Ok(ApiResponse<StockReportDto>.Ok(report));
        }
    }
}
