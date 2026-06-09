using ProjectBackend.BusinessLogic.Dto;

namespace ProjectBackend.BusinessLogic.Services
{
    public interface IWarehouseSyncService
    {
        /// <summary>
        /// Pulls stock levels from the warehouse provider and reconciles product stock.
        /// When <paramref name="dryRun"/> is true the changes are computed but not persisted.
        /// </summary>
        Task<WarehouseSyncResultDto> SyncAsync(bool dryRun, CancellationToken cancellationToken);
    }
}
