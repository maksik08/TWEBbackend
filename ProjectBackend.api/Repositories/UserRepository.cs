using Microsoft.EntityFrameworkCore;
using ProjectBackend.api.Data;
using ProjectBackend.api.Models.Common;
using ProjectBackend.api.Models.Domain;
using ProjectBackend.api.Models.Query;

namespace ProjectBackend.api.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ProjectDbContext _dbContext;

        public UserRepository(ProjectDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PagedResult<UserDomain>> GetAllAsync(UserQueryOptions queryOptions, CancellationToken cancellationToken)
        {
            var query = _dbContext.Users.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(queryOptions.Search))
            {
                query = query.Where(user =>
                    user.Username.Contains(queryOptions.Search) ||
                    user.Email.Contains(queryOptions.Search));
            }

            if (queryOptions.Role.HasValue)
            {
                query = query.Where(user => user.Role == queryOptions.Role.Value);
            }

            query = queryOptions.SortBy switch
            {
                "username" => queryOptions.SortDescending
                    ? query.OrderByDescending(user => user.Username).ThenBy(user => user.Id)
                    : query.OrderBy(user => user.Username).ThenBy(user => user.Id),
                "email" => queryOptions.SortDescending
                    ? query.OrderByDescending(user => user.Email).ThenBy(user => user.Id)
                    : query.OrderBy(user => user.Email).ThenBy(user => user.Id),
                "role" => queryOptions.SortDescending
                    ? query.OrderByDescending(user => user.Role).ThenBy(user => user.Id)
                    : query.OrderBy(user => user.Role).ThenBy(user => user.Id),
                _ => queryOptions.SortDescending
                    ? query.OrderByDescending(user => user.CreatedAt).ThenBy(user => user.Id)
                    : query.OrderBy(user => user.CreatedAt).ThenBy(user => user.Id)
            };

            return await query.ToPagedResultAsync(queryOptions, cancellationToken);
        }

        public async Task<UserDomain?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await _dbContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        }

        public async Task<UserDomain> CreateAsync(UserDomain user, CancellationToken cancellationToken)
        {
            await _dbContext.Users.AddAsync(user, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return user;
        }

        public async Task<UserDomain?> UpdateAsync(int id, UserDomain user, bool updatePassword, CancellationToken cancellationToken)
        {
            var existing = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
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

            await _dbContext.SaveChangesAsync(cancellationToken);
            return existing;
        }

        public async Task<UserDomain?> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            var existing = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
            if (existing is null)
            {
                return null;
            }

            _dbContext.Users.Remove(existing);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return existing;
        }

        public async Task<UserDomain?> GetByUsernameAsync(string username, CancellationToken cancellationToken)
        {
            return await _dbContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Username == username, cancellationToken);
        }

        public async Task<int> CountByRoleAsync(UserRole role, CancellationToken cancellationToken)
        {
            return await _dbContext.Users.CountAsync(u => u.Role == role, cancellationToken);
        }

        public async Task<bool> ExistsByEmailAsync(string email, int? excludedUserId, CancellationToken cancellationToken)
        {
            return await _dbContext.Users.AnyAsync(u =>
                u.Email == email &&
                (!excludedUserId.HasValue || u.Id != excludedUserId.Value), cancellationToken);
        }

        public async Task<bool> ExistsByUsernameAsync(string username, int? excludedUserId, CancellationToken cancellationToken)
        {
            return await _dbContext.Users.AnyAsync(u =>
                u.Username == username &&
                (!excludedUserId.HasValue || u.Id != excludedUserId.Value), cancellationToken);
        }
    }
}
