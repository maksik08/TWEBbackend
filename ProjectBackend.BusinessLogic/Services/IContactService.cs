using ProjectBackend.BusinessLogic.Dto;

namespace ProjectBackend.BusinessLogic.Services
{
    public interface IContactService
    {
        Task SubmitAsync(CreateContactMessageDto dto, CancellationToken cancellationToken);
    }
}
