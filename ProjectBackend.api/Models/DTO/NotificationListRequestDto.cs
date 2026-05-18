namespace ProjectBackend.api.Models.DTO
{
    public class NotificationListRequestDto : ListQueryRequestDto
    {
        public bool UnreadOnly { get; set; }
    }
}
