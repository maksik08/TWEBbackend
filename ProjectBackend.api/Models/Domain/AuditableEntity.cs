namespace ProjectBackend.api.Models.Domain
{
    public abstract class AuditableEntity
    {
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
