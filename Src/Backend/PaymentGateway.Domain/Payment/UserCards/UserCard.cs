namespace PaymentGateway.Domain.Payment.UserCards
{
    public class UserCard
    {
        public long Id { get; set; }
        public int MerchantId { get; set; }
        public string UserId { get; set; }
        public string Pan { get; set; }
        public string Cvv2 { get; set; }
        public string ExpYear { get; set; }
        public string ExpMonth { get; set; }
        public bool Displayable { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreateOn { get; set; }
    }
}
