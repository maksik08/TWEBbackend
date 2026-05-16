using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectBackend.api.Models.Domain
{
    public class NotificationDomain : AuditableEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public UserDomain? User { get; set; }

        [MaxLength(200)]
        public required string Title { get; set; }

        [MaxLength(1000)]
        public required string Message { get; set; }

        [MaxLength(100)]
        public string? RelatedEntityType { get; set; }

        public int? RelatedEntityId { get; set; }

        public bool IsRead { get; set; }
    }
}
