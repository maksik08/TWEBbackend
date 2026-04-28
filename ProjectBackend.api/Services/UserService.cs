using AutoMapper;
using ProjectBackend.api.Exceptions;
using ProjectBackend.api.Models.Domain;
using ProjectBackend.api.Models.DTO;
using ProjectBackend.api.Repositories;

namespace ProjectBackend.api.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;
        private readonly IMapper _mapper;

        public UserService(IUserRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<UserDto>> GetAllAsync()
        {
            var users = await _repository.GetAllAsync();
            return _mapper.Map<List<UserDto>>(users);
        }

        public async Task<UserDto> GetByIdAsync(int id)
        {
            var user = await _repository.GetByIdAsync(id);
            if (user is null)
            {
                throw new NotFoundException($"User with id {id} was not found.");
            }

            return _mapper.Map<UserDto>(user);
        }

        public async Task<UserDto> CreateAsync(CreateUserDto dto)
        {
            await EnsureUniqueCredentialsAsync(dto.Email, dto.Username);

            var entity = _mapper.Map<UserDomain>(dto);
            entity.Password = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            var created = await _repository.CreateAsync(entity);
            return _mapper.Map<UserDto>(created);
        }

        public async Task<UserDto> UpdateAsync(int id, UpdateUserDto dto)
        {
            await EnsureUniqueCredentialsAsync(dto.Email, dto.Username, id);

            var entity = _mapper.Map<UserDomain>(dto);
            var updatePassword = !string.IsNullOrWhiteSpace(dto.Password);
            entity.Password = updatePassword
                ? BCrypt.Net.BCrypt.HashPassword(dto.Password!)
                : string.Empty;

            var updated = await _repository.UpdateAsync(id, entity, updatePassword);
            if (updated is null)
            {
                throw new NotFoundException($"User with id {id} was not found.");
            }

            return _mapper.Map<UserDto>(updated);
        }

        public async Task<UserDto> DeleteAsync(int id)
        {
            var deleted = await _repository.DeleteAsync(id);
            if (deleted is null)
            {
                throw new NotFoundException($"User with id {id} was not found.");
            }

            return _mapper.Map<UserDto>(deleted);
        }

        private async Task EnsureUniqueCredentialsAsync(string email, string username, int? excludedUserId = null)
        {
            if (await _repository.ExistsByEmailAsync(email, excludedUserId))
            {
                throw new ConflictException("A user with this email already exists.");
            }

            if (await _repository.ExistsByUsernameAsync(username, excludedUserId))
            {
                throw new ConflictException("A user with this username already exists.");
            }
        }
    }
}
