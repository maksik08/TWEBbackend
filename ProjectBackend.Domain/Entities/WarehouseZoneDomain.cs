using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectBackend.Domain.Entities
{
    public class WarehouseZoneDomain : AuditableEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int WarehouseId { get; set; }

        [ForeignKey(nameof(WarehouseId))]
        public WarehouseDomain? Warehouse { get; set; }

        [MaxLength(120)]
        public required string Name { get; set; }

        [MaxLength(40)]
        public required string Code { get; set; }

        [MaxLength(300)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        public ICollection<ProductsDomain> Products { get; set; } = new List<ProductsDomain>();
    }
}
