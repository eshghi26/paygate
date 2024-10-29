namespace PaymentGateway.Domain.Payment.TokenInfos.Dto
{
    public class StartTransactionDto
    {
        public required string Token { get; set; }
        public string? Pan { get; set; }
        public string? Cvv2 { get; set; }
        public string? ExpYear { get; set; }
        public string? ExpMonth { get; set; }
        public bool SavePan { get; set; }
        public string? Captcha { get; set; }
    }
}