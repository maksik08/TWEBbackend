namespace ProjectBackend.BusinessLogic.Dto
{
    public class ActionLogDto : AuditableDto
    {
        public int Id { get; set; }
        public int? ActorUserId { get; set; }
        public string? ActorUserName { get; set; }
        public string ActorRole { get; set; } = string.Empty;
        public string EntityType { get; set; } = string.Empty;
        public int? EntityId { get; set; }
        public string Action { get; set; } = string.Empty;
        public string? Details { get; set; }
    }
}
