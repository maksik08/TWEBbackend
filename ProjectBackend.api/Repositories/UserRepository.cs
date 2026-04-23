using Microsoft.EntityFrameworkCore;
using ProjectBackend.api.Data;
using ProjectBackend.api.Models.Domain;

namespace ProjectBackend.api.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ProjectDbContext _dbContext;

        public UserRepository(ProjectDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<UserDomain> CreateAsync(UserDomain user)
        {
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();
            return user;
        }

        public async Task<UserDomain?> GetByUsernameAsync(string username)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            return await _dbContext.Users.AnyAsync(u => u.Email == email);
        }

        public async Task<bool> ExistsByUsernameAsync(string username)
        {
            return await _dbContext.Users.AnyAsync(u => u.Username == username);
        }
    }
}
