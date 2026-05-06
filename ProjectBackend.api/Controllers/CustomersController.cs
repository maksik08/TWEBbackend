using Microsoft.AspNetCore.Mvc;
using ProjectBackend.api.Filters;
using ProjectBackend.api.Models.DTO;
using ProjectBackend.api.Services;

namespace ProjectBackend.api.Controllers
{
    /// <summary>
    /// Customer endpoints.
    /// Read access: authenticated user or admin.
    /// Write access: admin.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [UserAccess]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomersController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        /// <summary>
        /// Returns a paged customer list with filtering and sorting.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] CustomerListRequestDto request, CancellationToken cancellationToken)
        {
            var customers = await _customerService.GetAllAsync(request, cancellationToken);
            return Ok(PagedResponse<CustomerDto>.Ok(customers));
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken cancellationToken)
        {
            var customer = await _customerService.GetByIdAsync(id, cancellationToken);
            return Ok(ApiResponse<CustomerDto>.Ok(customer));
        }

        [AdminMod]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCustomerDto dto, CancellationToken cancellationToken)
        {
            var created = await _customerService.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, ApiResponse<CustomerDto>.Ok(created, "Customer created successfully."));
        }

        [AdminMod]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateCustomerDto dto, CancellationToken cancellationToken)
        {
            var updated = await _customerService.UpdateAsync(id, dto, cancellationToken);
            return Ok(ApiResponse<CustomerDto>.Ok(updated, "Customer updated successfully."));
        }

        [AdminMod]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
        {
            var deleted = await _customerService.DeleteAsync(id, cancellationToken);
            return Ok(ApiResponse<CustomerDto>.Ok(deleted, "Customer deleted successfully."));
        }
    }
}
