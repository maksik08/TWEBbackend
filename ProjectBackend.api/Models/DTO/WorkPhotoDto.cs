namespace ProjectBackend.api.Models.DTO
{
    public class WorkPhotoDto : AuditableDto
    {
        public int Id { get; set; }

        public string FileName { get; set; } = string.Empty;

        public string FilePath { get; set; } = string.Empty;
    }
}
