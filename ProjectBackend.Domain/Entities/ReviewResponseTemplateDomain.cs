using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectBackend.Domain.Entities
{
    public class ReviewResponseTemplateDomain : AuditableEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(120)]
        public required string Name { get; set; }

        [MaxLength(2000)]
        public required string Body { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
