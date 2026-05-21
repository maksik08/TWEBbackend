using ProjectBackend.Domain.Common;
using ProjectBackend.BusinessLogic.Dto;

namespace ProjectBackend.BusinessLogic.Services
{
    public interface IManagerService
    {
        Task<PagedResult<OrderDto>> GetOrdersAsync(OrderListRequestDto request, CancellationToken cancellationToken);
        Task<OrderDto> GetOrderByIdAsync(int id, CancellationToken cancellationToken);
        Task<OrderDto> UpdateOrderStatusAsync(int id, UpdateOrderStatusDto dto, CancellationToken cancellationToken);
        Task<PagedResult<ServiceRequestDto>> GetRequestsAsync(ServiceRequestListRequestDto request, CancellationToken cancellationToken);
        Task<ServiceRequestDto> GetRequestByIdAsync(int id, CancellationToken cancellationToken);
        Task<IReadOnlyCollection<InstallerLookupDto>> GetInstallersAsync(CancellationToken cancellationToken);
        Task<ServiceRequestDto> AssignInstallerAsync(int id, AssignInstallerDto dto, CancellationToken cancellationToken);
        Task<ServiceRequestCommentDto> AddCommentAsync(int id, AddServiceRequestCommentDto dto, CancellationToken cancellationToken);
        Task<SalesReportDto> GetSalesReportAsync(CancellationToken cancellationToken);
        Task<ServiceRequestReportDto> GetServiceRequestReportAsync(CancellationToken cancellationToken);
        Task<StockReportDto> GetStockReportAsync(CancellationToken cancellationToken);
        Task<PagedResult<ProductDto>> GetProductsAsync(ProductListRequestDto request, CancellationToken cancellationToken);
        Task<ProductDto> UpdateProductVisibilityAsync(int id, UpdateProductVisibilityDto dto, CancellationToken cancellationToken);
    }
}
