using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectBackend.api.Models.Domain
{
    public class CategoryDomain : AuditableEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public required string Name { get; set; }

        public string? Description { get; set; }

        public ICollection<ProductsDomain> Products { get; set; } = new List<ProductsDomain>();
    }
}
