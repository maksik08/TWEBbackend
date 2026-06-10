using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectBackend.Domain.Entities
{
    public class KnowledgeBaseArticleDomain : AuditableEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(200)]
        public required string Title { get; set; }

        [MaxLength(120)]
        public required string Category { get; set; }

        [MaxLength(4000)]
        public required string Content { get; set; }

        public KnowledgeBaseArticleStatus Status { get; set; } = KnowledgeBaseArticleStatus.Published;
    }
}
