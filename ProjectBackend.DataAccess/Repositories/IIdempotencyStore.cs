using ProjectBackend.Domain.Entities;

namespace ProjectBackend.DataAccess.Repositories
{
    public interface IIdempotencyStore
    {
        Task<IdempotencyRecordDomain?> GetAsync(int? userId, string key, string method, string path, CancellationToken cancellationToken);

        Task<bool> TrySaveAsync(IdempotencyRecordDomain record, CancellationToken cancellationToken);
    }
}
