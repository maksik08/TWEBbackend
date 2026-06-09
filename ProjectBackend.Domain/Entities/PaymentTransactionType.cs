namespace ProjectBackend.Domain.Entities
{
    public enum PaymentTransactionType
    {
        BalanceTopUp = 0,
        OrderPayment = 1,
        Refund = 2,
        ServicePayment = 3
    }
}
