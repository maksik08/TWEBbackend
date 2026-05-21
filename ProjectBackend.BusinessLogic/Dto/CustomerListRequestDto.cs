namespace ProjectBackend.BusinessLogic.Dto
{
    public class CustomerListRequestDto : ListQueryRequestDto
    {
        public string? Search { get; set; }
    }
}
