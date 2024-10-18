using PaymentGateway.Domain.Payment.UserCards;

namespace PaymentGateway.Domain.Payment.TokenInfos.Dto
{
    public class GetTokenInfoResponse
    {
        public decimal Amount { get; set; }
        public DateTime ExpireDate { get; set; }
        public string? CaptchaAddress { get; set; }
        public List<UserCard>? Cards { get; set; }
    }
}
