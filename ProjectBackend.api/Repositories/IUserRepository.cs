using ProjectBackend.api.Models.Domain;

namespace ProjectBackend.api.Repositories
{
    public interface IUserRepository
    {
        Task<UserDomain> CreateAsync(UserDomain user);
        Task<bool> ExistsByEmailAsync(string email);
        Task<bool> ExistsByUsernameAsync(string username);
    }
}
