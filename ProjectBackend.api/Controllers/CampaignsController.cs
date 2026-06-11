using Microsoft.AspNetCore.Mvc;
using ProjectBackend.api.Filters;
using ProjectBackend.BusinessLogic.Dto;
using ProjectBackend.BusinessLogic.Services;

namespace ProjectBackend.api.Controllers
{
    [Route("api/admin/campaigns")]
    [ApiController]
    [AdminMod]
    public class CampaignsController : ControllerBase
    {
        private readonly ICampaignService _service;
        public CampaignsController(ICampaignService service) { _service = service; }

        [HttpGet] public async Task<IActionResult> GetAll([FromQuery]int page=1,[FromQuery]int pageSize=20,CancellationToken ct=default) => Ok(PagedResponse<CampaignDto>.Ok(await _service.GetAllAsync(page,pageSize,ct)));
        [HttpPost] public async Task<IActionResult> Create([FromBody] CreateCampaignDto dto, CancellationToken ct) => Ok(ApiResponse<CampaignDto>.Ok(await _service.CreateAsync(dto, ct), "Campaign created."));
    }
}
