using ProjectBackend.Domain.Common;
using ProjectBackend.BusinessLogic.Dto;

namespace ProjectBackend.BusinessLogic.Services
{
    public interface IUserService
    {
        Task<PagedResult<UserDto>> GetAllAsync(UserListRequestDto request, CancellationToken cancellationToken);
        Task<UserDto> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<UserDto> CreateAsync(CreateUserDto dto, CancellationToken cancellationToken);
        Task<UserDto> UpdateAsync(int id, UpdateUserDto dto, CancellationToken cancellationToken);
        Task<UserDto> DeleteAsync(int id, CancellationToken cancellationToken);
        Task<UserDto> SetBlockedAsync(int id, bool isBlocked, CancellationToken cancellationToken);
        Task<UserDto> UpdateProfileAsync(int id, UpdateProfileDto dto, CancellationToken cancellationToken);
        Task<UserDto> TopUpBalanceAsync(int id, decimal amount, CancellationToken cancellationToken);
    }
}
