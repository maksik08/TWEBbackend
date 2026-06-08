using ProjectBackend.Domain.Entities;

namespace ProjectBackend.BusinessLogic.Dto
{
    public class ContactMessageDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public int? UserId { get; set; }
        public string? UserName { get; set; }
        public bool IsRead { get; set; }
        public ContactMessageStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
