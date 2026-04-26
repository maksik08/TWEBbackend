using ProjectBackend.api.Models.DTO;

namespace ProjectBackend.api.Services
{
    public interface IUserService
    {
        Task<List<UserDto>> GetAllAsync();
        Task<UserDto?> GetByIdAsync(int id);
        Task<UserDto> CreateAsync(CreateUserDto dto);
        Task<UserDto?> UpdateAsync(int id, UpdateUserDto dto);
        Task<UserDto?> DeleteAsync(int id);
    }
}
