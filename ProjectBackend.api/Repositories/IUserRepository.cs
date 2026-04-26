using ProjectBackend.api.Models.Domain;

namespace ProjectBackend.api.Repositories
{
    public interface IUserRepository
    {
        Task<List<UserDomain>> GetAllAsync();
        Task<UserDomain?> GetByIdAsync(int id);
        Task<UserDomain> CreateAsync(UserDomain user);
        Task<UserDomain?> UpdateAsync(int id, UserDomain user, bool updatePassword);
        Task<UserDomain?> DeleteAsync(int id);
        Task<UserDomain?> GetByUsernameAsync(string username);
        Task<bool> ExistsByEmailAsync(string email, int? excludedUserId = null);
        Task<bool> ExistsByUsernameAsync(string username, int? excludedUserId = null);
    }
}
