using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectBackend.api.Models.Domain
{
    public class IdempotencyRecordDomain
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public required string Key { get; set; }

        public int? UserId { get; set; }

        public required string Method { get; set; }

        public required string Path { get; set; }

        public int StatusCode { get; set; }

        public required string ContentType { get; set; }

        public required string ResponseBody { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime ExpiresAt { get; set; }
    }
}
