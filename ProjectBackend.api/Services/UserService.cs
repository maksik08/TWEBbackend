using AutoMapper;
using ProjectBackend.api.Exceptions;
using ProjectBackend.api.Models.Common;
using ProjectBackend.api.Models.Domain;
using ProjectBackend.api.Models.DTO;
using ProjectBackend.api.Models.Query;
using ProjectBackend.api.Repositories;

namespace ProjectBackend.api.Services
{
    public class UserService : ApplicationServiceBase, IUserService
    {
        private readonly IUserRepository _repository;
        private readonly IMapper _mapper;
        private readonly ICurrentUserContext _currentUserContext;

        public UserService(IUserRepository repository, IMapper mapper, ICurrentUserContext currentUserContext)
        {
            _repository = repository;
            _mapper = mapper;
            _currentUserContext = currentUserContext;
        }

        public async Task<PagedResult<UserDto>> GetAllAsync(UserListRequestDto request, CancellationToken cancellationToken)
        {
            var queryOptions = new UserQueryOptions
            {
                Page = QueryValidationHelper.NormalizePage(request.Page),
                PageSize = QueryValidationHelper.NormalizePageSize(request.PageSize),
                SortBy = QueryValidationHelper.NormalizeSortBy(request.SortBy, "createdat", "username", "email", "createdat", "role"),
                SortDescending = QueryValidationHelper.NormalizeSortDescending(request.SortDirection),
                Search = QueryValidationHelper.NormalizeSearch(request.Search),
                Role = request.Role
            };

            var users = await _repository.GetAllAsync(queryOptions, cancellationToken);
            return new PagedResult<UserDto>
            {
                Items = _mapper.Map<IReadOnlyCollection<UserDto>>(users.Items),
                TotalCount = users.TotalCount,
                Page = users.Page,
                PageSize = users.PageSize
            };
        }

        public async Task<UserDto> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            var user = EnsureFound(await _repository.GetByIdAsync(id, cancellationToken), "User", id);
            return _mapper.Map<UserDto>(user);
        }

        public async Task<UserDto> CreateAsync(CreateUserDto dto, CancellationToken cancellationToken)
        {
            var email = NormalizeEmail(dto.Email);
            var username = NormalizeRequiredText(dto.Username, "Username");
            await EnsureUniqueCredentialsAsync(email, username, null, cancellationToken);

            var entity = _mapper.Map<UserDomain>(dto);
            entity.Email = email;
            entity.Username = username;
            entity.Password = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            var created = await _repository.CreateAsync(entity, cancellationToken);
            return _mapper.Map<UserDto>(created);
        }

        public async Task<UserDto> UpdateAsync(int id, UpdateUserDto dto, CancellationToken cancellationToken)
        {
            var existingUser = EnsureFound(await _repository.GetByIdAsync(id, cancellationToken), "User", id);
            var email = NormalizeEmail(dto.Email);
            var username = NormalizeRequiredText(dto.Username, "Username");

            await EnsureUniqueCredentialsAsync(email, username, id, cancellationToken);

            if (_currentUserContext.UserId == id && existingUser.Role != dto.Role)
            {
                throw new ValidationException("You cannot change your own role.");
            }

            var entity = _mapper.Map<UserDomain>(dto);
            entity.Email = email;
            entity.Username = username;
            var updatePassword = !string.IsNullOrWhiteSpace(dto.Password);
            if (updatePassword)
            {
                entity.Password = BCrypt.Net.BCrypt.HashPassword(dto.Password!);
            }

            var updated = await _repository.UpdateAsync(id, entity, updatePassword, cancellationToken);
            updated = EnsureFound(updated, "User", id);
            return _mapper.Map<UserDto>(updated);
        }

        public async Task<UserDto> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            var user = EnsureFound(await _repository.GetByIdAsync(id, cancellationToken), "User", id);

            if (user.Role == UserRole.Admin)
            {
                var adminCount = await _repository.CountByRoleAsync(UserRole.Admin, cancellationToken);
                if (adminCount <= 1)
                {
                    throw new ValidationException("The last administrator cannot be deleted.");
                }
            }

            var deleted = EnsureFound(await _repository.DeleteAsync(id, cancellationToken), "User", id);
            return _mapper.Map<UserDto>(deleted);
        }

        public async Task<UserDto> UpdateProfileAsync(int id, UpdateProfileDto dto, CancellationToken cancellationToken)
        {
            EnsureFound(await _repository.GetByIdAsync(id, cancellationToken), "User", id);

            var firstName = NormalizeOptionalText(dto.FirstName);
            var lastName = NormalizeOptionalText(dto.LastName);
            var phone = NormalizeOptionalText(dto.Phone);

            var updated = await _repository.UpdateProfileAsync(id, firstName, lastName, phone, cancellationToken);
            updated = EnsureFound(updated, "User", id);
            return _mapper.Map<UserDto>(updated);
        }

        public async Task<UserDto> TopUpBalanceAsync(int id, decimal amount, CancellationToken cancellationToken)
        {
            if (amount <= 0)
            {
                throw new ValidationException("Amount must be greater than zero.");
            }

            EnsureFound(await _repository.GetByIdAsync(id, cancellationToken), "User", id);

            var updated = await _repository.AdjustBalanceAsync(id, amount, cancellationToken);
            updated = EnsureFound(updated, "User", id);
            return _mapper.Map<UserDto>(updated);
        }

        private async Task EnsureUniqueCredentialsAsync(
            string email,
            string username,
            int? excludedUserId,
            CancellationToken cancellationToken)
        {
            if (await _repository.ExistsByEmailAsync(email, excludedUserId, cancellationToken))
            {
                throw new ConflictException("A user with this email already exists.");
            }

            if (await _repository.ExistsByUsernameAsync(username, excludedUserId, cancellationToken))
            {
                throw new ConflictException("A user with this username already exists.");
            }
        }
    }
}
