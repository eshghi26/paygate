using System.Text.Json.Serialization;

namespace PaymentGateway.PaymentApi.PaymentType.Concrete.Aqayepardakht.Dto
{
    public class AqTokenResponse
    {
        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("code")]
        public string? ErrorCode { get; set; }

        [JsonPropertyName("transid")]
        public string? TransactionId { get; set; }
    }
}
