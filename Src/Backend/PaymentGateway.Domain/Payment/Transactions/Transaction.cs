namespace PaymentGateway.Domain.Payment.Transactions
{
    public class Transaction
    {
        public long Id { get; set; }
        public long? ParentId { get; set; }
        public short Status { get; set; }
        public short OperationType { get; set; }
        public decimal Amount { get; set; }
        public int MerchantId { get; set; }
        public int GatewayId { get; set; }
        public Guid Token { get; set; }
        public string TransactionCode { get; set; }
        public string? ReserveNumber { get; set; }
        public string? InvoiceNumber { get; set; }
        public decimal WageAmount { get; set; }
        public DateTime? GetOtpDate { get; set; }
        public DateTime? FinishTransactionDate { get; set; }
        public DateTime? CancelTransactionDate { get; set; }
        public DateTime? VerifyDate { get; set; }
        public int? VerifyRetryCount { get; set; }
        public decimal? FinalAmount { get; set; }
        public string? Description { get; set; }
        public DateTime CreateOn { get; set; }
        public string? MerchantUserId { get; set; }
        public string TrackingNumber { get; set; }
    }
}
