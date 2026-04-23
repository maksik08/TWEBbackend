using AutoMapper;
using ProjectBackend.api.Models.Domain;
using ProjectBackend.api.Models.DTO;
using ProjectBackend.api.Repositories;

namespace ProjectBackend.api.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _repository;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public AuthService(IUserRepository repository, ITokenService tokenService, IMapper mapper)
        {
            _repository = repository;
            _tokenService = tokenService;
            _mapper = mapper;
        }

        public async Task<(AuthResponseDto? Response, string? Error)> RegisterAsync(RegisterDto dto)
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
            return (BuildResponse(created), null);
        }

        public async Task<(AuthResponseDto? Response, string? Error)> LoginAsync(LoginDto dto)
        {
            var user = await _repository.GetByUsernameAsync(dto.Username);
            if (user is null || user.Password != dto.Password)
                return (null, "Invalid username or password.");

            return (BuildResponse(user), null);
        }

        private AuthResponseDto BuildResponse(UserDomain user)
        {
            return new AuthResponseDto
            {
                Token = _tokenService.CreateToken(user),
                User = _mapper.Map<UserDto>(user)
            };
        }
    }
}
