namespace PaymentGateway.Domain.Payment.Gateways
{
    public class Gateway
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Key { get; set; }
        public int MerchantId { get; set; }
        public short Type { get; set; }
        public decimal MinAmount { get; set; }
        public decimal MaxAmount { get; set; }
        public short Status { get; set; }
        public short ModuleType { get; set; }
        public string? Pin { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? TerminalNumber { get; set; }
        public string? CallbackUrl { get; set; }
        public string? BankCallbackUrl { get; set; }
        public string? Comment { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreateOn { get; set; }
    }
}
