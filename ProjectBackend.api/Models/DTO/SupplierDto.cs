namespace ProjectBackend.api.Models.DTO
{
    public class SupplierDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? ContactEmail { get; set; }
        public string? Phone { get; set; }
    }
}
