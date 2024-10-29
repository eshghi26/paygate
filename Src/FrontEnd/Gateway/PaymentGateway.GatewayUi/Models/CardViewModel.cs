namespace PaymentGateway.GatewayUi.Models
{
    public class CardViewModel
    {
        public long Id { get; set; }
        public string? Pan { get; set; }
        public string? Cvv2 { get; set; }
        public string? ExpYear { get; set; }
        public string? ExpMonth { get; set; }
    }
}