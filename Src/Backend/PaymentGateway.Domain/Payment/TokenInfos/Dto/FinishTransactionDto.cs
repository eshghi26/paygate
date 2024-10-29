namespace PaymentGateway.Domain.Payment.TokenInfos.Dto
{
    public class FinishTransactionDto
    {
        public required string Token { get; set; }
        public required string Pin { get; set; }
    }
}