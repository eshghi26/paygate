namespace PaymentGateway.PaymentApi.PaymentType.Factory
{
    public class GetTransactionCodeModel
    {
        public string? TransactionCode { get; set; }
        public string? GatewayToken { get; set; }
        public string? ReserveNumber { get; set; }
        public string? Captcha { get; set; }
    }
}
