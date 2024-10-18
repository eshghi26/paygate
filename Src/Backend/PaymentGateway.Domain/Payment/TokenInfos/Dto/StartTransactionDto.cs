namespace PaymentGateway.Domain.Payment.TokenInfos.Dto
{
    public class StartTransactionDto
    {
        public required string Token { get; set; }
        public string? Pan { get; set; }
        public string? ExpYear { get; set; }
        public string? ExpMonth { get; set; }
        public bool SaveCard { get; set; }
        public string? Captcha { get; set; }
    }
}