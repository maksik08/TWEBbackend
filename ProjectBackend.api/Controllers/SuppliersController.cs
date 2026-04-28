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
        public async Task<IActionResult> GetAll()
        {
            var suppliers = await _supplierService.GetAllAsync();
            return Ok(suppliers);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var supplier = await _supplierService.GetByIdAsync(id);
            return Ok(supplier);
        }

        [AdminMod]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateSupplierDto dto)
        {
            var created = await _supplierService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [AdminMod]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateSupplierDto dto)
        {
            var updated = await _supplierService.UpdateAsync(id, dto);
            return Ok(updated);
        }

        [AdminMod]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var deleted = await _supplierService.DeleteAsync(id);
            return Ok(deleted);
        }
    }
}
