namespace PaymentGateway.Domain.Payment.Transactions.Dto
{
    public class VerifyTransactionDto
    {
        public string? Key { get; set; }
        public decimal Amount { get; set; }
        public string? Token { get; set; }
    }
}
