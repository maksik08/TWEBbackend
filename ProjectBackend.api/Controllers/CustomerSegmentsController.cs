using Microsoft.AspNetCore.Mvc;
using ProjectBackend.api.Filters;
using ProjectBackend.BusinessLogic.Dto;
using ProjectBackend.BusinessLogic.Services;

namespace ProjectBackend.api.Controllers
{
    [Route("api/admin/customer-segments")]
    [ApiController]
    [AdminMod]
    public class CustomerSegmentsController : ControllerBase
    {
        private readonly ICustomerSegmentService _service;
        public CustomerSegmentsController(ICustomerSegmentService service) { _service = service; }

        [HttpGet] public async Task<IActionResult> GetAll([FromQuery]int page=1,[FromQuery]int pageSize=20,CancellationToken ct=default) => Ok(PagedResponse<CustomerSegmentDto>.Ok(await _service.GetAllAsync(page,pageSize,ct)));
        [HttpPost] public async Task<IActionResult> Create([FromBody] CreateCustomerSegmentDto dto, CancellationToken ct) => Ok(ApiResponse<CustomerSegmentDto>.Ok(await _service.CreateAsync(dto, ct), "Segment created."));
        [HttpPost("{id:int}/assign")] public async Task<IActionResult> Assign(int id, [FromBody] AssignSegmentDto dto, CancellationToken ct) { await _service.AssignAsync(id, dto.CustomerId, ct); return Ok(ApiResponse<object?>.Ok(null, "Assigned.")); }
    }
}
