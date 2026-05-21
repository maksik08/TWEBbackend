namespace ProjectBackend.Domain.Entities
{
    public enum PaymentTransactionStatus
    {
        Pending = 0,
        Completed = 1,
        Failed = 2,
        Cancelled = 3
    }
}
