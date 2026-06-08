using AutoMapper;
using ProjectBackend.BusinessLogic.Exceptions;
using ProjectBackend.Domain.Common;
using ProjectBackend.BusinessLogic.Common;
using ProjectBackend.Domain.Entities;
using ProjectBackend.BusinessLogic.Dto;
using ProjectBackend.DataAccess.Repositories;

namespace ProjectBackend.BusinessLogic.Services
{
    public class AuthService : ApplicationServiceBase, IAuthService
    {
        private readonly IUserRepository _repository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IPasswordResetTokenRepository _passwordResetTokenRepository;
        private readonly ITokenService _tokenService;
        private readonly IPasswordResetNotifier _passwordResetNotifier;
        private readonly IMapper _mapper;

        public AuthService(
            IUserRepository repository,
            IRefreshTokenRepository refreshTokenRepository,
            IPasswordResetTokenRepository passwordResetTokenRepository,
            ITokenService tokenService,
            IPasswordResetNotifier passwordResetNotifier,
            IMapper mapper)
        {
            _repository = repository;
            _refreshTokenRepository = refreshTokenRepository;
            _passwordResetTokenRepository = passwordResetTokenRepository;
            _tokenService = tokenService;
            _passwordResetNotifier = passwordResetNotifier;
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

            if (user.IsBlocked)
            {
                throw new UnauthorizedAppException("This account has been blocked. Please contact support.");
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

            if (user.IsBlocked)
            {
                throw new UnauthorizedAppException("This account has been blocked. Please contact support.");
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

        public async Task ForgotPasswordAsync(ForgotPasswordDto dto, CancellationToken cancellationToken)
        {
            var email = NormalizeEmail(dto.Email);
            var user = await _repository.GetByEmailAsync(email, cancellationToken);
            if (user is null)
            {
                return;
            }

            await _passwordResetTokenRepository.InvalidateActiveForUserAsync(user.Id, cancellationToken);

            var (rawToken, tokenHash, expiresAt) = _tokenService.CreatePasswordResetToken();
            await _passwordResetTokenRepository.CreateAsync(new PasswordResetTokenDomain
            {
                TokenHash = tokenHash,
                UserId = user.Id,
                ExpiresAt = expiresAt,
                CreatedAt = DateTime.UtcNow
            }, cancellationToken);

            await _passwordResetNotifier.NotifyAsync(user, rawToken, cancellationToken);
        }

        public async Task ResetPasswordAsync(ResetPasswordDto dto, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(dto.Token))
            {
                throw new ValidationException("Reset token is required.");
            }

            if (dto.NewPassword != dto.ConfirmPassword)
            {
                throw new ValidationException("Passwords do not match.");
            }

            var hash = _tokenService.HashPasswordResetToken(dto.Token);
            var stored = await _passwordResetTokenRepository.GetByHashAsync(hash, cancellationToken);
            if (stored is null || !stored.IsUsable)
            {
                throw new ValidationException("Reset token is invalid or expired.");
            }

            var user = await _repository.GetByIdAsync(stored.UserId, cancellationToken);
            if (user is null)
            {
                throw new ValidationException("Reset token is invalid or expired.");
            }

            var newHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            await _repository.UpdatePasswordAsync(user.Id, newHash, cancellationToken);

            stored.ConsumedAt = DateTime.UtcNow;
            await _passwordResetTokenRepository.UpdateAsync(stored, cancellationToken);

            await _refreshTokenRepository.RevokeAllForUserAsync(user.Id, cancellationToken);
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
