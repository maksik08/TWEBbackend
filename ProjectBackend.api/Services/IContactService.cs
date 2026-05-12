using ProjectBackend.api.Models.DTO;

namespace ProjectBackend.api.Services
{
    public interface IContactService
    {
        Task SubmitAsync(CreateContactMessageDto dto, CancellationToken cancellationToken);
    }
}
