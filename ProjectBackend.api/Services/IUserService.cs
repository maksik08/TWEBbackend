using ProjectBackend.api.Models.Common;
using ProjectBackend.api.Models.DTO;

namespace ProjectBackend.api.Services
{
    public interface IUserService
    {
        Task<PagedResult<UserDto>> GetAllAsync(UserListRequestDto request, CancellationToken cancellationToken);
        Task<UserDto> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<UserDto> CreateAsync(CreateUserDto dto, CancellationToken cancellationToken);
        Task<UserDto> UpdateAsync(int id, UpdateUserDto dto, CancellationToken cancellationToken);
        Task<UserDto> DeleteAsync(int id, CancellationToken cancellationToken);
    }
}
