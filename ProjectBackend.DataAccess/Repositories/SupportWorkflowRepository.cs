using Microsoft.EntityFrameworkCore;
using ProjectBackend.Domain.Common;
using ProjectBackend.Domain.Entities;
using ProjectBackend.Domain.Query;

namespace ProjectBackend.DataAccess.Repositories
{
    public class SupportWorkflowRepository : ISupportWorkflowRepository
    {
        private readonly ProjectDbContext _dbContext;

        public SupportWorkflowRepository(ProjectDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PagedResult<KnowledgeBaseArticleDomain>> GetArticlesAsync(KnowledgeBaseArticleQueryOptions queryOptions, CancellationToken cancellationToken)
        {
            var query = _dbContext.KnowledgeBaseArticles.AsNoTracking().AsQueryable();

            if (!queryOptions.IncludeDrafts)
            {
                query = query.Where(article => article.Status == KnowledgeBaseArticleStatus.Published);
            }

            if (queryOptions.Status.HasValue)
            {
                query = query.Where(article => article.Status == queryOptions.Status.Value);
            }

            if (!string.IsNullOrWhiteSpace(queryOptions.Category))
            {
                query = query.Where(article => article.Category == queryOptions.Category);
            }

            if (!string.IsNullOrWhiteSpace(queryOptions.Search))
            {
                query = query.Where(article => article.Title.Contains(queryOptions.Search) || article.Content.Contains(queryOptions.Search));
            }

            query = queryOptions.SortDescending
                ? query.OrderByDescending(article => article.UpdatedAt).ThenByDescending(article => article.Id)
                : query.OrderBy(article => article.Title).ThenBy(article => article.Id);

            return await query.ToPagedResultAsync(queryOptions, cancellationToken);
        }

        public async Task<KnowledgeBaseArticleDomain?> GetArticleAsync(int id, CancellationToken cancellationToken)
        {
            return await _dbContext.KnowledgeBaseArticles.FirstOrDefaultAsync(article => article.Id == id, cancellationToken);
        }

        public async Task<KnowledgeBaseArticleDomain> CreateArticleAsync(KnowledgeBaseArticleDomain article, CancellationToken cancellationToken)
        {
            await _dbContext.KnowledgeBaseArticles.AddAsync(article, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return article;
        }

        public async Task<KnowledgeBaseArticleDomain> UpdateArticleAsync(KnowledgeBaseArticleDomain article, CancellationToken cancellationToken)
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
            return article;
        }

        public async Task<WarrantyClaimDomain> CreateWarrantyClaimAsync(WarrantyClaimDomain claim, CancellationToken cancellationToken)
        {
            await _dbContext.WarrantyClaims.AddAsync(claim, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            await _dbContext.Entry(claim).Reference(item => item.Customer).LoadAsync(cancellationToken);
            await _dbContext.Entry(claim).Reference(item => item.Product).LoadAsync(cancellationToken);
            return claim;
        }

        public async Task<WarrantyClaimDomain?> GetWarrantyClaimAsync(int id, CancellationToken cancellationToken)
        {
            return await _dbContext.WarrantyClaims
                .Include(claim => claim.Customer)
                .Include(claim => claim.Product)
                .FirstOrDefaultAsync(claim => claim.Id == id, cancellationToken);
        }

        public async Task<WarrantyClaimDomain> UpdateWarrantyClaimAsync(WarrantyClaimDomain claim, CancellationToken cancellationToken)
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
            return claim;
        }

        public async Task<RemoteDiagnosticSessionDomain> CreateDiagnosticSessionAsync(RemoteDiagnosticSessionDomain session, CancellationToken cancellationToken)
        {
            await _dbContext.RemoteDiagnosticSessions.AddAsync(session, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            await _dbContext.Entry(session).Reference(item => item.Agent).LoadAsync(cancellationToken);
            return session;
        }

        public async Task<RemoteDiagnosticSessionDomain?> GetDiagnosticSessionAsync(int id, CancellationToken cancellationToken)
        {
            return await _dbContext.RemoteDiagnosticSessions
                .Include(session => session.Agent)
                .FirstOrDefaultAsync(session => session.Id == id, cancellationToken);
        }

        public async Task<RemoteDiagnosticSessionDomain> UpdateDiagnosticSessionAsync(RemoteDiagnosticSessionDomain session, CancellationToken cancellationToken)
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
            return session;
        }
    }
}
