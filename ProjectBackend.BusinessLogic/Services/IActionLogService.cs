namespace ProjectBackend.BusinessLogic.Services
{
    public interface IActionLogService
    {
        Task RecordAsync(string action, string entityType, int? entityId, string? details, CancellationToken cancellationToken);
    }
}
