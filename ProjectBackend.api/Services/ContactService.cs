using ProjectBackend.api.Models.Domain;
using ProjectBackend.api.Models.DTO;
using ProjectBackend.api.Repositories;

namespace ProjectBackend.api.Services
{
    public class ContactService : ApplicationServiceBase, IContactService
    {
        private readonly IContactMessageRepository _repository;
        private readonly ICurrentUserContext _currentUser;
        private readonly ILogger<ContactService> _logger;

        public ContactService(
            IContactMessageRepository repository,
            ICurrentUserContext currentUser,
            ILogger<ContactService> logger)
        {
            _repository = repository;
            _currentUser = currentUser;
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
    }
}
