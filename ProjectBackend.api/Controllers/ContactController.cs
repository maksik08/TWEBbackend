using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
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
    }
}
