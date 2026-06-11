namespace ProjectBackend.BusinessLogic.Dto
{
    public class CampaignDto { public int Id { get; set; } public string Name { get; set; } = string.Empty; public string? Description { get; set; } public string? CouponCode { get; set; } public DateTime StartsAt { get; set; } public DateTime? EndsAt { get; set; } public bool IsActive { get; set; } }
    public class CreateCampaignDto { public string Name { get; set; } = string.Empty; public string? Description { get; set; } public string? CouponCode { get; set; } public DateTime StartsAt { get; set; } public DateTime? EndsAt { get; set; } public bool IsActive { get; set; } = true; }
}
