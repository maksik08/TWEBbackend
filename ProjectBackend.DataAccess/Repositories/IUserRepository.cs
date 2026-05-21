using ProjectBackend.Domain.Common;
using ProjectBackend.Domain.Entities;
using ProjectBackend.Domain.Query;

namespace ProjectBackend.DataAccess.Repositories
{
    public interface IUserRepository
    {
        Task<PagedResult<UserDomain>> GetAllAsync(UserQueryOptions queryOptions, CancellationToken cancellationToken);
        Task<UserDomain?> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<UserDomain> CreateAsync(UserDomain user, CancellationToken cancellationToken);
        Task<UserDomain?> UpdateAsync(int id, UserDomain user, bool updatePassword, CancellationToken cancellationToken);
        Task<UserDomain?> DeleteAsync(int id, CancellationToken cancellationToken);
        Task<UserDomain?> GetByUsernameAsync(string username, CancellationToken cancellationToken);
        Task<UserDomain?> GetByEmailAsync(string email, CancellationToken cancellationToken);
        Task UpdatePasswordAsync(int userId, string passwordHash, CancellationToken cancellationToken);
        Task<IReadOnlyCollection<UserDomain>> GetByRoleAsync(UserRole role, CancellationToken cancellationToken);
        Task<int> CountByRoleAsync(UserRole role, CancellationToken cancellationToken);
        Task<bool> ExistsByEmailAsync(string email, int? excludedUserId, CancellationToken cancellationToken);
        Task<bool> ExistsByUsernameAsync(string username, int? excludedUserId, CancellationToken cancellationToken);
        Task<UserDomain?> UpdateProfileAsync(int id, string? firstName, string? lastName, string? phone, CancellationToken cancellationToken);
        Task<UserDomain?> AdjustBalanceAsync(int id, decimal delta, CancellationToken cancellationToken);
    }
}
