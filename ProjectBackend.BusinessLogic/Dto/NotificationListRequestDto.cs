namespace ProjectBackend.BusinessLogic.Dto
{
    public class NotificationListRequestDto : ListQueryRequestDto
    {
        public bool UnreadOnly { get; set; }
    }
}
