using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectBackend.Domain.Entities
{
    public enum LoyaltyTransactionType { Earn = 0, Redeem = 1, Adjustment = 2 }

    public class LoyaltyTransactionDomain : AuditableEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int LoyaltyCardId { get; set; }
        public int UserId { get; set; }
        public int Points { get; set; }
        public LoyaltyTransactionType Type { get; set; }
        [MaxLength(1000)]
        public string? Note { get; set; }
    }
}
