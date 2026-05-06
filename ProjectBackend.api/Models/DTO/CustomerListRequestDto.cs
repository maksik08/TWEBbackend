namespace ProjectBackend.api.Models.DTO
{
    public class CustomerListRequestDto : ListQueryRequestDto
    {
        public string? Search { get; set; }
    }
}
