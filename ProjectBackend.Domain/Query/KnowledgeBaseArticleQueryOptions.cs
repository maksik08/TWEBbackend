using ProjectBackend.Domain.Entities;

namespace ProjectBackend.Domain.Query
{
    public sealed class KnowledgeBaseArticleQueryOptions : PagedQueryOptions
    {
        public string? Search { get; init; }
        public string? Category { get; init; }
        public KnowledgeBaseArticleStatus? Status { get; init; }
        public bool IncludeDrafts { get; init; }
    }
}
