namespace PaymentGateway.Domain.Payment.TokenInfos
{
    public class TokenInfo
    {
        public int Id { get; set; }
        public Guid Token { get; set; }
        public decimal Amount { get; set; }
        public string CallBackUrl { get; set; }
        public string? TrCode { get; set; }
        public string? ReserveNumber { get; set; }
        public DateTime ExpireDate { get; set; }
        public int MerchantId { get; set; }
        public int GatewayId { get; set; }
        public DateTime CreateOn { get; set; }
        public string? MerchantUserId { get; set; }
        public string? Captcha { get; set; }
    }
}
