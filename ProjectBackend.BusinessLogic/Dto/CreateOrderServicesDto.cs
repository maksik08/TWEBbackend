using System.ComponentModel.DataAnnotations;

namespace ProjectBackend.BusinessLogic.Dto
{
    /// <summary>
    /// Optional installation-service parameters for an order.
    /// The equipment-installation portion and the final price are recomputed on the server
    /// from the order's real product prices — the client cannot dictate the services total.
    /// </summary>
    public class CreateOrderServicesDto
    {
        [Required]
        [StringLength(40)]
        public string ObjectType { get; set; } = string.Empty;

        [Required]
        [StringLength(40)]
        public string InstallationType { get; set; } = string.Empty;

        public List<string> Works { get; set; } = new();

        [Range(0, 1000)]
        public int StaffCount { get; set; }

        [Range(0, 100000)]
        public decimal StaffRate { get; set; }

        [Range(0, 1000000)]
        public decimal InstallationCost { get; set; }

        [Range(0, 1000000)]
        public decimal DeliveryCost { get; set; }
    }
}
