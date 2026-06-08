using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using ProjectBackend.api.Filters;
using ProjectBackend.BusinessLogic.Dto;
using ProjectBackend.BusinessLogic.Services;

namespace ProjectBackend.api.Controllers
{
    /// <summary>
    /// Public contact form. Anyone (guest or authenticated) can submit a message.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        private readonly IContactService _contactService;

        public ContactController(IContactService contactService)
        {
            _contactService = contactService;
        }

        /// <summary>
        /// Submits a contact message. Rate-limited to prevent spam.
        /// </summary>
        [HttpPost]
        [EnableRateLimiting("contact")]
        public async Task<IActionResult> Submit([FromBody] CreateContactMessageDto dto, CancellationToken cancellationToken)
        {
            await _contactService.SubmitAsync(dto, cancellationToken);
            return Ok(ApiResponse<object?>.Ok(null, "Your message has been received. We will get back to you shortly."));
        }

        /// <summary>
        /// Lists submitted contact messages. Access: admin only.
        /// </summary>
        [AdminMod]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] ContactMessageListRequestDto request, CancellationToken cancellationToken)
        {
            var messages = await _contactService.GetAllAsync(request, cancellationToken);
            return Ok(PagedResponse<ContactMessageDto>.Ok(messages));
        }

        /// <summary>
        /// Returns a single contact message. Access: admin only.
        /// </summary>
        [AdminMod]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken cancellationToken)
        {
            var message = await _contactService.GetByIdAsync(id, cancellationToken);
            return Ok(ApiResponse<ContactMessageDto>.Ok(message));
        }

        /// <summary>
        /// Updates the triage status of a contact message. Access: admin only.
        /// </summary>
        [AdminMod]
        [HttpPatch("{id:int}/status")]
        public async Task<IActionResult> UpdateStatus([FromRoute] int id, [FromBody] UpdateContactMessageStatusDto dto, CancellationToken cancellationToken)
        {
            var message = await _contactService.UpdateStatusAsync(id, dto, cancellationToken);
            return Ok(ApiResponse<ContactMessageDto>.Ok(message, "Message status updated."));
        }
    }
}
