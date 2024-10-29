namespace PaymentGateway.GatewayUi.Models
{
    public class UserCardsModel
    {
        public DateTime ExpireDate { get; set; }

        public CardViewModel[]? Cards { get; set; }
    }
}