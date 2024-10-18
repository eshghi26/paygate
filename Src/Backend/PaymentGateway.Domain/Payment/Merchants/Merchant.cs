namespace PaymentGateway.Domain.Payment.Merchants
{
    public class Merchant
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Logo { get; set; }
        public string? ThumbnailLogo { get; set; }
        public string? Comment { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreateOn { get; set; }
        public decimal WageLimit { get; set; }
    }
}
