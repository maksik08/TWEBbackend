using Microsoft.AspNetCore.Mvc;
using ProjectBackend.api.Filters;
using ProjectBackend.api.Models.DTO;
using ProjectBackend.api.Services;

namespace ProjectBackend.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [UserAccess]
    public class SuppliersController : ControllerBase
    {
        private readonly ISupplierService _supplierService;

        public SuppliersController(ISupplierService supplierService)
        {
            _supplierService = supplierService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] SupplierListRequestDto request, CancellationToken cancellationToken)
        {
            var suppliers = await _supplierService.GetAllAsync(request, cancellationToken);
            return Ok(PagedResponse<SupplierDto>.Ok(suppliers));
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken cancellationToken)
        {
            var supplier = await _supplierService.GetByIdAsync(id, cancellationToken);
            return Ok(ApiResponse<SupplierDto>.Ok(supplier));
        }

        [AdminMod]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateSupplierDto dto, CancellationToken cancellationToken)
        {
            var created = await _supplierService.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, ApiResponse<SupplierDto>.Ok(created, "Supplier created successfully."));
        }

        [AdminMod]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateSupplierDto dto, CancellationToken cancellationToken)
        {
            var updated = await _supplierService.UpdateAsync(id, dto, cancellationToken);
            return Ok(ApiResponse<SupplierDto>.Ok(updated, "Supplier updated successfully."));
        }

        [AdminMod]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
        {
            var deleted = await _supplierService.DeleteAsync(id, cancellationToken);
            return Ok(ApiResponse<SupplierDto>.Ok(deleted, "Supplier deleted successfully."));
        }
    }
}
