using System.Text.Json.Serialization;

namespace PaymentGateway.PaymentApi.PaymentType.Concrete.Aqayepardakht.Dto
{
    public class AqVerifyResponse
    {
        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("code")]
        public string? Code { get; set; }
    }
}
