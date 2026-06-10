using ProjectBackend.BusinessLogic.Dto;
using ProjectBackend.Domain.Common;

namespace ProjectBackend.BusinessLogic.Services
{
    public interface ISupportWorkflowService
    {
        Task<PagedResult<KnowledgeBaseArticleDto>> GetArticlesAsync(KnowledgeBaseArticleListRequestDto request, CancellationToken cancellationToken);
        Task<KnowledgeBaseArticleDto> CreateArticleAsync(CreateKnowledgeBaseArticleDto dto, CancellationToken cancellationToken);
        Task<KnowledgeBaseArticleDto> UpdateArticleAsync(int id, UpdateKnowledgeBaseArticleDto dto, CancellationToken cancellationToken);
        Task<WarrantyCheckDto> CheckWarrantyAsync(int productId, int? orderId, CancellationToken cancellationToken);
        Task<WarrantyClaimDto> CreateWarrantyClaimAsync(CreateWarrantyClaimDto dto, CancellationToken cancellationToken);
        Task<WarrantyClaimDto> UpdateWarrantyClaimAsync(int id, UpdateWarrantyClaimStatusDto dto, CancellationToken cancellationToken);
        Task<RemoteDiagnosticSessionDto> StartDiagnosticAsync(int ticketId, StartRemoteDiagnosticDto dto, CancellationToken cancellationToken);
        Task<RemoteDiagnosticSessionDto> CompleteDiagnosticAsync(int id, CompleteRemoteDiagnosticDto dto, CancellationToken cancellationToken);
    }
}
