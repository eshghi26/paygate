﻿namespace PaymentGateway.Domain.Payment.Transactions.Dto
{
    public class GetPaymentInfoResponse
    {
        public string Token { get; set; }
        public short Status { get; set; }
        public decimal Amount { get; set; }
        public decimal? FinalAmount { get; set; }
        public string? InvoiceNumber { get; set; }
        public string? TrackingNumber { get; set; }
        public string CallbackUrl { get; set; }
    }
}
