using Microsoft.AspNetCore.Mvc;
using ProjectBackend.api.Filters;
using ProjectBackend.api.Models.DTO;
using ProjectBackend.api.Services;

namespace ProjectBackend.api.Controllers
{
    /// <summary>
    /// Payment history endpoints.
    /// Access: authenticated user for own payments, admin for all payments.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [UserAccess]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentTransactionService _paymentTransactionService;

        public PaymentsController(IPaymentTransactionService paymentTransactionService)
        {
            _paymentTransactionService = paymentTransactionService;
        }

        [HttpGet("my")]
        public async Task<IActionResult> GetMy([FromQuery] PaymentTransactionListRequestDto request, CancellationToken cancellationToken)
        {
            var payments = await _paymentTransactionService.GetMyPaymentsAsync(request, cancellationToken);
            return Ok(PagedResponse<PaymentTransactionDto>.Ok(payments));
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken cancellationToken)
        {
            var payment = await _paymentTransactionService.GetByIdAsync(id, cancellationToken);
            return Ok(ApiResponse<PaymentTransactionDto>.Ok(payment));
        }

        [AdminMod]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PaymentTransactionListRequestDto request, CancellationToken cancellationToken)
        {
            var payments = await _paymentTransactionService.GetAllAsync(request, cancellationToken);
            return Ok(PagedResponse<PaymentTransactionDto>.Ok(payments));
        }
    }
}
