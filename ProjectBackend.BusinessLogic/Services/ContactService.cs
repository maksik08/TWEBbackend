using AutoMapper;
using ProjectBackend.Domain.Common;
using ProjectBackend.Domain.Entities;
using ProjectBackend.Domain.Query;
using ProjectBackend.BusinessLogic.Dto;
using ProjectBackend.DataAccess.Repositories;

namespace ProjectBackend.BusinessLogic.Services
{
    public class ContactService : ApplicationServiceBase, IContactService
    {
        private readonly IContactMessageRepository _repository;
        private readonly ICurrentUserContext _currentUser;
        private readonly IMapper _mapper;
        private readonly ILogger<ContactService> _logger;

        public ContactService(
            IContactMessageRepository repository,
            ICurrentUserContext currentUser,
            IMapper mapper,
            ILogger<ContactService> logger)
        {
            _repository = repository;
            _currentUser = currentUser;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task SubmitAsync(CreateContactMessageDto dto, CancellationToken cancellationToken)
        {
            var entity = new ContactMessageDomain
            {
                Name = NormalizeRequiredText(dto.Name, "Name"),
                Email = NormalizeEmail(dto.Email),
                Subject = NormalizeRequiredText(dto.Subject, "Subject"),
                Message = NormalizeRequiredText(dto.Message, "Message"),
                UserId = _currentUser.UserId,
                Status = ContactMessageStatus.New,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _repository.CreateAsync(entity, cancellationToken);

            _logger.LogInformation(
                "Contact message #{Id} from {Name} <{Email}>: {Subject}",
                created.Id,
                created.Name,
                created.Email,
                created.Subject);
        }

        public async Task<PagedResult<ContactMessageDto>> GetAllAsync(ContactMessageListRequestDto request, CancellationToken cancellationToken)
        {
            var queryOptions = new ContactMessageQueryOptions
            {
                Page = QueryValidationHelper.NormalizePage(request.Page),
                PageSize = QueryValidationHelper.NormalizePageSize(request.PageSize),
                SortBy = QueryValidationHelper.NormalizeSortBy(request.SortBy, "createdat", "createdat"),
                SortDescending = QueryValidationHelper.NormalizeSortDescending(request.SortDirection ?? "desc"),
                Search = QueryValidationHelper.NormalizeSearch(request.Search),
                Status = request.Status
            };

            var messages = await _repository.GetAllAsync(queryOptions, cancellationToken);
            return new PagedResult<ContactMessageDto>
            {
                Items = _mapper.Map<IReadOnlyCollection<ContactMessageDto>>(messages.Items),
                TotalCount = messages.TotalCount,
                Page = messages.Page,
                PageSize = messages.PageSize
            };
        }

        public async Task<ContactMessageDto> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            var message = EnsureFound(await _repository.GetByIdAsync(id, cancellationToken), "Contact message", id);
            return _mapper.Map<ContactMessageDto>(message);
        }

        public async Task<ContactMessageDto> UpdateStatusAsync(int id, UpdateContactMessageStatusDto dto, CancellationToken cancellationToken)
        {
            var updated = EnsureFound(
                await _repository.UpdateStatusAsync(id, dto.Status, cancellationToken),
                "Contact message",
                id);

            return _mapper.Map<ContactMessageDto>(updated);
        }
    }
}
