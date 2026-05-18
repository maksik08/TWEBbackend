using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectBackend.api.Models.Domain
{
    public class UserDomain : AuditableEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public required string Email { get; set; }

        public required string Username { get; set; }

        public required string Password { get; set; }

        public UserRole Role { get; set; } = UserRole.Customer;

        [MaxLength(100)]
        public string? FirstName { get; set; }

        [MaxLength(100)]
        public string? LastName { get; set; }

        [MaxLength(50)]
        public string? Phone { get; set; }

        public decimal Balance { get; set; }

        public ICollection<OrderDomain> Orders { get; set; } = new List<OrderDomain>();

        public ICollection<ServiceRequestDomain> ServiceRequests { get; set; } = new List<ServiceRequestDomain>();

        public ICollection<ServiceRequestDomain> ManagedServiceRequests { get; set; } = new List<ServiceRequestDomain>();

        public ICollection<ServiceRequestDomain> AssignedServiceRequests { get; set; } = new List<ServiceRequestDomain>();

        public ICollection<ServiceRequestCommentDomain> ServiceRequestComments { get; set; } = new List<ServiceRequestCommentDomain>();

        public ICollection<NotificationDomain> Notifications { get; set; } = new List<NotificationDomain>();

        public ICollection<ActionLogDomain> ActionLogs { get; set; } = new List<ActionLogDomain>();

        public ICollection<PaymentTransactionDomain> PaymentTransactions { get; set; } = new List<PaymentTransactionDomain>();
    }
}
