using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectBackend.Domain.Entities
{
    public class ActionLogDomain : AuditableEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int? ActorUserId { get; set; }

        [ForeignKey(nameof(ActorUserId))]
        public UserDomain? ActorUser { get; set; }

        [MaxLength(50)]
        public required string ActorRole { get; set; }

        [MaxLength(100)]
        public required string EntityType { get; set; }

        public int? EntityId { get; set; }

        [MaxLength(100)]
        public required string Action { get; set; }

        [MaxLength(2000)]
        public string? Details { get; set; }
    }
}
