namespace PaymentGateway.Domain.Payment.Resources
{
    public class Resource
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public byte[] FileData { get; set; }
        public DateTime CreateOn { get; set; }
        public DateTime ExpireDate { get; set; }
    }
}
