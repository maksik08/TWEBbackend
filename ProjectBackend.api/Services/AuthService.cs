using AutoMapper;
using ProjectBackend.api.Exceptions;
using ProjectBackend.api.Models.Common;
using ProjectBackend.api.Models.Domain;
using ProjectBackend.api.Models.DTO;
using ProjectBackend.api.Repositories;

namespace ProjectBackend.api.Services
{
    public class AuthService : ApplicationServiceBase, IAuthService
    {
        private readonly IUserRepository _repository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public AuthService(
            IUserRepository repository,
            IRefreshTokenRepository refreshTokenRepository,
            ITokenService tokenService,
            IMapper mapper)
        {
            _repository = repository;
            _refreshTokenRepository = refreshTokenRepository;
            _tokenService = tokenService;
            _mapper = mapper;
        }

        public async Task<AuthResult> RegisterAsync(RegisterDto dto, CancellationToken cancellationToken)
        {
            var email = NormalizeEmail(dto.Email);
            var username = NormalizeRequiredText(dto.Username, "Username");

            if (await _repository.ExistsByEmailAsync(email, null, cancellationToken))
            {
                throw new ConflictException("Email is already registered.");
            }

            if (await _repository.ExistsByUsernameAsync(username, null, cancellationToken))
            {
                throw new ConflictException("Username is already taken.");
            }

            var entity = new UserDomain
            {
                Email = email,
                Username = username,
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = UserRole.User
            };

            var created = await _repository.CreateAsync(entity, cancellationToken);
            return await BuildAuthResultAsync(created, cancellationToken);
        }

        public async Task<AuthResult> LoginAsync(LoginDto dto, CancellationToken cancellationToken)
        {
            var username = NormalizeRequiredText(dto.Username, "Username");
            var user = await _repository.GetByUsernameAsync(username, cancellationToken);
            if (user is null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.Password))
            {
                throw new UnauthorizedAppException("Invalid username or password.");
            }

            return await BuildAuthResultAsync(user, cancellationToken);
        }

        public async Task<AuthResult> RefreshAsync(string rawRefreshToken, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(rawRefreshToken))
            {
                throw new UnauthorizedAppException("Refresh token is missing.");
            }

            var hash = _tokenService.HashRefreshToken(rawRefreshToken);
            var stored = await _refreshTokenRepository.GetByHashAsync(hash, cancellationToken);
            if (stored is null || !stored.IsActive)
            {
                throw new UnauthorizedAppException("Refresh token is invalid or expired.");
            }

            var user = await _repository.GetByIdAsync(stored.UserId, cancellationToken);
            if (user is null)
            {
                throw new UnauthorizedAppException("User no longer exists.");
            }

            stored.RevokedAt = DateTime.UtcNow;
            await _refreshTokenRepository.UpdateAsync(stored, cancellationToken);

            return await BuildAuthResultAsync(user, cancellationToken);
        }

        public async Task LogoutAsync(string rawRefreshToken, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(rawRefreshToken))
            {
                return;
            }

            var hash = _tokenService.HashRefreshToken(rawRefreshToken);
            var stored = await _refreshTokenRepository.GetByHashAsync(hash, cancellationToken);
            if (stored is null || stored.RevokedAt is not null)
            {
                return;
            }

            stored.RevokedAt = DateTime.UtcNow;
            await _refreshTokenRepository.UpdateAsync(stored, cancellationToken);
        }

        private async Task<AuthResult> BuildAuthResultAsync(UserDomain user, CancellationToken cancellationToken)
        {
            var (rawToken, tokenHash, refreshExpiresAt) = _tokenService.CreateRefreshToken();

            await _refreshTokenRepository.CreateAsync(new RefreshTokenDomain
            {
                TokenHash = tokenHash,
                UserId = user.Id,
                ExpiresAt = refreshExpiresAt,
                CreatedAt = DateTime.UtcNow
            }, cancellationToken);

            return new AuthResult
            {
                Response = new AuthResponseDto
                {
                    Token = _tokenService.CreateAccessToken(user),
                    User = _mapper.Map<UserDto>(user)
                },
                RefreshToken = rawToken,
                RefreshTokenExpiresAt = refreshExpiresAt
            };
        }
    }
}
