using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectBackend.Domain.Entities
{
    public class WarehouseDocumentDomain : AuditableEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public WarehouseDocumentType Type { get; set; }

        [MaxLength(40)]
        public required string Number { get; set; }

        [MaxLength(120)]
        public string? RelatedEntityType { get; set; }

        public int? RelatedEntityId { get; set; }

        [MaxLength(200)]
        public required string Title { get; set; }

        public required string Content { get; set; }
    }
}
