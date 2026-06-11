using Microsoft.AspNetCore.Mvc;
using ProjectBackend.api.Filters;
using ProjectBackend.BusinessLogic.Dto;
using ProjectBackend.BusinessLogic.Services;

namespace ProjectBackend.api.Controllers
{
    /// <summary>
    /// Service tariffs (the catalogue customers choose from).
    /// Listing: public (guest). Management: admin only.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceTariffsController : ControllerBase
    {
        private readonly IServiceTariffService _serviceTariffService;

        public ServiceTariffsController(IServiceTariffService serviceTariffService)
        {
            _serviceTariffService = serviceTariffService;
        }

        /// <summary>
        /// Lists service tariffs. By default only active ones (for the public customer catalogue);
        /// pass includeInactive=true for the admin management view (active + inactive).
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] bool includeInactive, CancellationToken cancellationToken)
        {
            var tariffs = await _serviceTariffService.GetAllAsync(activeOnly: !includeInactive, cancellationToken);
            return Ok(ApiResponse<IReadOnlyCollection<ServiceTariffDto>>.Ok(tariffs));
        }

        [AdminMod]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateServiceTariffDto dto, CancellationToken cancellationToken)
        {
            var created = await _serviceTariffService.CreateAsync(dto, cancellationToken);
            return Ok(ApiResponse<ServiceTariffDto>.Ok(created, "Service tariff created."));
        }

        [AdminMod]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateServiceTariffDto dto, CancellationToken cancellationToken)
        {
            var updated = await _serviceTariffService.UpdateAsync(id, dto, cancellationToken);
            return Ok(ApiResponse<ServiceTariffDto>.Ok(updated, "Service tariff updated."));
        }

        [AdminMod]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
        {
            await _serviceTariffService.DeleteAsync(id, cancellationToken);
            return Ok(ApiResponse<object?>.Ok(null, "Service tariff deleted."));
        }
    }
}
