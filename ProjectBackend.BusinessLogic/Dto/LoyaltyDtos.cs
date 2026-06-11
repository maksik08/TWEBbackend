namespace ProjectBackend.BusinessLogic.Dto
{
    public class LoyaltyBalanceDto { public int UserId { get; set; } public int Points { get; set; } }
    public class LoyaltyTransactionDto { public int Id { get; set; } public int Points { get; set; } public string Type { get; set; } = string.Empty; public string? Note { get; set; } public DateTime CreatedAt { get; set; } }
    public class EarnPointsRequestDto { public int Points { get; set; } public string? Note { get; set; } }
    public class RedeemPointsRequestDto { public int Points { get; set; } public string? Note { get; set; } }
}
