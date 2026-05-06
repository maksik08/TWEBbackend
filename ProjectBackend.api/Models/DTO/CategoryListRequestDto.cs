namespace ProjectBackend.api.Models.DTO
{
    public class CategoryListRequestDto : ListQueryRequestDto
    {
        public string? Search { get; set; }
    }
}
