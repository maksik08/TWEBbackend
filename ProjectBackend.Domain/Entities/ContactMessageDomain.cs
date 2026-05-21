using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectBackend.Domain.Entities
{
    public class ContactMessageDomain
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public required string Name { get; set; }

        public required string Email { get; set; }

        public required string Subject { get; set; }

        public required string Message { get; set; }

        public int? UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public UserDomain? User { get; set; }

        public bool IsRead { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
