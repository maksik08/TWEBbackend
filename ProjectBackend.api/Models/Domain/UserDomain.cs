using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectBackend.api.Models.Domain
{
    public class UserDomain
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public required string Email { get; set; }

        public required string Username { get; set; }

        public required string Password { get; set; }

        public UserRole Role { get; set; } = UserRole.Customer;


        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
