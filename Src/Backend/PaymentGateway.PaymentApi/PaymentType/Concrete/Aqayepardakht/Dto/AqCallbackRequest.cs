using System.Text.Json.Serialization;

namespace PaymentGateway.PaymentApi.PaymentType.Concrete.Aqayepardakht.Dto
{
    public class AqCallbackRequest
    {
        public string? Transid { get; set; }
        
        public string? Cardnumber { get; set; }

        public string? Tracking_number { get; set; }

        public string? Invoice_id { get; set; }

        public string? Bank { get; set; }

        public string? Status { get; set; }
    }
}
