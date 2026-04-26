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

        public async Task<List<UserDomain>> GetAllAsync()
        {
            return await _dbContext.Users
                .AsNoTracking()
                .OrderByDescending(u => u.CreatedAt)
                .ToListAsync();
        }

        public async Task<UserDomain?> GetByIdAsync(int id)
        {
            return await _dbContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<UserDomain> CreateAsync(UserDomain user)
        {
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();
            return user;
        }

        public async Task<UserDomain?> UpdateAsync(int id, UserDomain user, bool updatePassword)
        {
            var existing = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (existing is null)
            {
                return null;
            }

            existing.Email = user.Email;
            existing.Username = user.Username;
            existing.Role = user.Role;

            if (updatePassword)
            {
                existing.Password = user.Password;
            }

            await _dbContext.SaveChangesAsync();
            return existing;
        }

        public async Task<UserDomain?> DeleteAsync(int id)
        {
            var existing = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (existing is null)
            {
                return null;
            }

            _dbContext.Users.Remove(existing);
            await _dbContext.SaveChangesAsync();
            return existing;
        }

        public async Task<UserDomain?> GetByUsernameAsync(string username)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<bool> ExistsByEmailAsync(string email, int? excludedUserId = null)
        {
            return await _dbContext.Users.AnyAsync(u =>
                u.Email == email &&
                (!excludedUserId.HasValue || u.Id != excludedUserId.Value));
        }

        public async Task<bool> ExistsByUsernameAsync(string username, int? excludedUserId = null)
        {
            return await _dbContext.Users.AnyAsync(u =>
                u.Username == username &&
                (!excludedUserId.HasValue || u.Id != excludedUserId.Value));
        }
    }
}
