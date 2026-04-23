using AutoMapper;
using ProjectBackend.api.Models.Domain;
using ProjectBackend.api.Models.DTO;
using ProjectBackend.api.Repositories;

namespace ProjectBackend.api.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _repository;
        private readonly IMapper _mapper;

        public AuthService(IUserRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<(UserDto? User, string? Error)> RegisterAsync(RegisterDto dto)
        {
            if (await _repository.ExistsByEmailAsync(dto.Email))
                return (null, "Email is already registered.");

            if (await _repository.ExistsByUsernameAsync(dto.Username))
                return (null, "Username is already taken.");

            var entity = new UserDomain
            {
                Email = dto.Email,
                Username = dto.Username,
                Password = dto.Password
            };

            var created = await _repository.CreateAsync(entity);
            return (_mapper.Map<UserDto>(created), null);
        }
    }
}
