namespace ProjectBackend.BusinessLogic.Dto
{
    public class SupplierDto : AuditableDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? ContactEmail { get; set; }
        public string? Phone { get; set; }
    }
}
