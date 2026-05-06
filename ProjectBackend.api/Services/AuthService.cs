using AutoMapper;
using ProjectBackend.api.Exceptions;
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

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto, CancellationToken cancellationToken)
        {
            if (await _repository.ExistsByEmailAsync(dto.Email, null, cancellationToken))
            {
                throw new ConflictException("Email is already registered.");
            }

            if (await _repository.ExistsByUsernameAsync(dto.Username, null, cancellationToken))
            {
                throw new ConflictException("Username is already taken.");
            }

            var entity = new UserDomain
            {
                Email = dto.Email,
                Username = dto.Username,
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = UserRole.User
            };

            var created = await _repository.CreateAsync(entity, cancellationToken);
            return BuildResponse(created);
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto dto, CancellationToken cancellationToken)
        {
            var user = await _repository.GetByUsernameAsync(dto.Username, cancellationToken);
            if (user is null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.Password))
            {
                throw new UnauthorizedAppException("Invalid username or password.");
            }

            return BuildResponse(user);
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
