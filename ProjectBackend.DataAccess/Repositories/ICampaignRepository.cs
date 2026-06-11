using ProjectBackend.Domain.Common;
using ProjectBackend.Domain.Entities;

namespace ProjectBackend.DataAccess.Repositories
{
    public interface ICampaignRepository
    {
        Task<PagedResult<CampaignDomain>> GetAllAsync(PagedQueryOptions options, CancellationToken cancellationToken);
        Task<CampaignDomain?> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<CampaignDomain> CreateAsync(CampaignDomain campaign, CancellationToken cancellationToken);
        Task<CampaignDomain?> UpdateAsync(CampaignDomain campaign, CancellationToken cancellationToken);
        Task<CampaignDomain?> DeleteAsync(int id, CancellationToken cancellationToken);
    }
}
