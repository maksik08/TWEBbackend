using AutoMapper;
using ProjectBackend.api.Exceptions;
using ProjectBackend.api.Models.Common;
using ProjectBackend.api.Models.Domain;
using ProjectBackend.api.Models.DTO;
using ProjectBackend.api.Models.Query;
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
            var user = await _repository.GetByIdAsync(id, cancellationToken);
            if (user is null)
            {
                throw new NotFoundException($"User with id {id} was not found.");
            }

            return _mapper.Map<UserDto>(user);
        }

        public async Task<UserDto> CreateAsync(CreateUserDto dto, CancellationToken cancellationToken)
        {
            await EnsureUniqueCredentialsAsync(dto.Email, dto.Username, null, cancellationToken);

            var entity = _mapper.Map<UserDomain>(dto);
            entity.Password = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            var created = await _repository.CreateAsync(entity, cancellationToken);
            return _mapper.Map<UserDto>(created);
        }

        public async Task<UserDto> UpdateAsync(int id, UpdateUserDto dto, CancellationToken cancellationToken)
        {
            await EnsureUniqueCredentialsAsync(dto.Email, dto.Username, id, cancellationToken);

            var entity = _mapper.Map<UserDomain>(dto);
            var updatePassword = !string.IsNullOrWhiteSpace(dto.Password);
            if (updatePassword)
            {
                entity.Password = BCrypt.Net.BCrypt.HashPassword(dto.Password!);
            }

            var updated = await _repository.UpdateAsync(id, entity, updatePassword, cancellationToken);
            if (updated is null)
            {
                throw new NotFoundException($"User with id {id} was not found.");
            }

            return _mapper.Map<UserDto>(updated);
        }

        public async Task<UserDto> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            var deleted = await _repository.DeleteAsync(id, cancellationToken);
            if (deleted is null)
            {
                throw new NotFoundException($"User with id {id} was not found.");
            }

            return _mapper.Map<UserDto>(deleted);
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
