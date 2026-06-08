namespace ProjectBackend.BusinessLogic.Dto
{
    public class ActionLogListRequestDto : ListQueryRequestDto
    {
        public string? Search { get; set; }

        public string? EntityType { get; set; }

        public string? Action { get; set; }
    }
}
