using ProjectBackend.api.Models.Common;
using ProjectBackend.api.Models.DTO;

namespace ProjectBackend.api.Services
{
    public interface IOrderService
    {
        Task<PagedResult<OrderDto>> GetMineAsync(OrderListRequestDto request, CancellationToken cancellationToken);
        Task<PagedResult<OrderDto>> GetAllAsync(OrderListRequestDto request, CancellationToken cancellationToken);
        Task<OrderDto> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<OrderDto> CreateAsync(CreateOrderDto dto, CancellationToken cancellationToken);
        Task<OrderDto> PayAsync(int id, CancellationToken cancellationToken);
        Task<OrderDto> CancelAsync(int id, CancellationToken cancellationToken);
    }
}
