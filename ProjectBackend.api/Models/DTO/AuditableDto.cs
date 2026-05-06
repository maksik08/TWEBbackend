namespace ProjectBackend.api.Models.DTO
{
    public abstract class AuditableDto
    {
        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
