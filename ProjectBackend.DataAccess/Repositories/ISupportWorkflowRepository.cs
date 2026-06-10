using ProjectBackend.Domain.Common;
using ProjectBackend.Domain.Entities;
using ProjectBackend.Domain.Query;

namespace ProjectBackend.DataAccess.Repositories
{
    public interface ISupportWorkflowRepository
    {
        Task<PagedResult<KnowledgeBaseArticleDomain>> GetArticlesAsync(KnowledgeBaseArticleQueryOptions queryOptions, CancellationToken cancellationToken);
        Task<KnowledgeBaseArticleDomain?> GetArticleAsync(int id, CancellationToken cancellationToken);
        Task<KnowledgeBaseArticleDomain> CreateArticleAsync(KnowledgeBaseArticleDomain article, CancellationToken cancellationToken);
        Task<KnowledgeBaseArticleDomain> UpdateArticleAsync(KnowledgeBaseArticleDomain article, CancellationToken cancellationToken);
        Task<WarrantyClaimDomain> CreateWarrantyClaimAsync(WarrantyClaimDomain claim, CancellationToken cancellationToken);
        Task<WarrantyClaimDomain?> GetWarrantyClaimAsync(int id, CancellationToken cancellationToken);
        Task<WarrantyClaimDomain> UpdateWarrantyClaimAsync(WarrantyClaimDomain claim, CancellationToken cancellationToken);
        Task<RemoteDiagnosticSessionDomain> CreateDiagnosticSessionAsync(RemoteDiagnosticSessionDomain session, CancellationToken cancellationToken);
        Task<RemoteDiagnosticSessionDomain?> GetDiagnosticSessionAsync(int id, CancellationToken cancellationToken);
        Task<RemoteDiagnosticSessionDomain> UpdateDiagnosticSessionAsync(RemoteDiagnosticSessionDomain session, CancellationToken cancellationToken);
    }
}
