using ProjectBackend.api.Models.Common;
using ProjectBackend.api.Models.Domain;
using ProjectBackend.api.Models.Query;

namespace ProjectBackend.api.Repositories
{
    public interface IUserRepository
    {
        Task<PagedResult<UserDomain>> GetAllAsync(UserQueryOptions queryOptions, CancellationToken cancellationToken);
        Task<UserDomain?> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<UserDomain> CreateAsync(UserDomain user, CancellationToken cancellationToken);
        Task<UserDomain?> UpdateAsync(int id, UserDomain user, bool updatePassword, CancellationToken cancellationToken);
        Task<UserDomain?> DeleteAsync(int id, CancellationToken cancellationToken);
        Task<UserDomain?> GetByUsernameAsync(string username, CancellationToken cancellationToken);
        Task<bool> ExistsByEmailAsync(string email, int? excludedUserId, CancellationToken cancellationToken);
        Task<bool> ExistsByUsernameAsync(string username, int? excludedUserId, CancellationToken cancellationToken);
    }
}
