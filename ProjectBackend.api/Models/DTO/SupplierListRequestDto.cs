namespace ProjectBackend.api.Models.DTO
{
    public class SupplierListRequestDto : ListQueryRequestDto
    {
        public string? Search { get; set; }
    }
}
