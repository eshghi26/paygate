namespace PaymentGateway.Domain.Payment.TokenInfos.Dto
{
    public class CreateTokenRequestDto
    {
        public string? Key { get; set; }
        public decimal Amount { get; set; }
        public string? Callback { get; set; }
        public string? UserId { get; set; }
    }
}