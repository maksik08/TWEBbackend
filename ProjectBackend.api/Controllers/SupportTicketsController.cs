using Microsoft.AspNetCore.Mvc;
using ProjectBackend.api.Filters;
using ProjectBackend.BusinessLogic.Dto;
using ProjectBackend.BusinessLogic.Services;

namespace ProjectBackend.api.Controllers
{
    /// <summary>
    /// Support ticketing. Customers open tickets and exchange messages with support agents.
    /// Listing all tickets, changing status and assigning agents require support staff (Support or Admin).
    /// </summary>
    [Route("api/support/tickets")]
    [ApiController]
    [UserAccess]
    public class SupportTicketsController : ControllerBase
    {
        private readonly ISupportTicketService _supportTicketService;

        public SupportTicketsController(ISupportTicketService supportTicketService)
        {
            _supportTicketService = supportTicketService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateSupportTicketDto dto, CancellationToken cancellationToken)
        {
            var ticket = await _supportTicketService.CreateAsync(dto, cancellationToken);
            return Ok(ApiResponse<SupportTicketDto>.Ok(ticket, "Support ticket created."));
        }

        [HttpGet("my")]
        public async Task<IActionResult> GetMine([FromQuery] SupportTicketListRequestDto request, CancellationToken cancellationToken)
        {
            var tickets = await _supportTicketService.GetMineAsync(request, cancellationToken);
            return Ok(PagedResponse<SupportTicketDto>.Ok(tickets));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] SupportTicketListRequestDto request, CancellationToken cancellationToken)
        {
            var tickets = await _supportTicketService.GetAllAsync(request, cancellationToken);
            return Ok(PagedResponse<SupportTicketDto>.Ok(tickets));
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken cancellationToken)
        {
            var ticket = await _supportTicketService.GetByIdAsync(id, cancellationToken);
            return Ok(ApiResponse<SupportTicketDto>.Ok(ticket));
        }

        [HttpPost("{id:int}/messages")]
        public async Task<IActionResult> PostMessage([FromRoute] int id, [FromBody] PostSupportMessageDto dto, CancellationToken cancellationToken)
        {
            var ticket = await _supportTicketService.PostMessageAsync(id, dto, cancellationToken);
            return Ok(ApiResponse<SupportTicketDto>.Ok(ticket, "Message sent."));
        }

        [HttpPatch("{id:int}/status")]
        public async Task<IActionResult> UpdateStatus([FromRoute] int id, [FromBody] UpdateSupportTicketStatusDto dto, CancellationToken cancellationToken)
        {
            var ticket = await _supportTicketService.UpdateStatusAsync(id, dto, cancellationToken);
            return Ok(ApiResponse<SupportTicketDto>.Ok(ticket, "Ticket status updated."));
        }

        [HttpPost("{id:int}/assign")]
        public async Task<IActionResult> Assign([FromRoute] int id, [FromBody] AssignSupportAgentDto dto, CancellationToken cancellationToken)
        {
            var ticket = await _supportTicketService.AssignAsync(id, dto, cancellationToken);
            return Ok(ApiResponse<SupportTicketDto>.Ok(ticket, "Agent assigned."));
        }
    }
}
