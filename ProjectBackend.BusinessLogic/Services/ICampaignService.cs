using ProjectBackend.BusinessLogic.Dto;
using ProjectBackend.Domain.Common;

namespace ProjectBackend.BusinessLogic.Services
{
    public interface ICampaignService
    {
        Task<PagedResult<CampaignDto>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken);
        Task<CampaignDto> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<CampaignDto> CreateAsync(CreateCampaignDto dto, CancellationToken cancellationToken);
        Task<CampaignDto> UpdateAsync(int id, CreateCampaignDto dto, CancellationToken cancellationToken);
        Task DeleteAsync(int id, CancellationToken cancellationToken);
    }
}
