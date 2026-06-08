using ProjectBackend.Domain.Common;
using ProjectBackend.BusinessLogic.Dto;

namespace ProjectBackend.BusinessLogic.Services
{
    public interface IContactService
    {
        Task SubmitAsync(CreateContactMessageDto dto, CancellationToken cancellationToken);

        Task<PagedResult<ContactMessageDto>> GetAllAsync(ContactMessageListRequestDto request, CancellationToken cancellationToken);

        Task<ContactMessageDto> GetByIdAsync(int id, CancellationToken cancellationToken);

        Task<ContactMessageDto> UpdateStatusAsync(int id, UpdateContactMessageStatusDto dto, CancellationToken cancellationToken);
    }
}
