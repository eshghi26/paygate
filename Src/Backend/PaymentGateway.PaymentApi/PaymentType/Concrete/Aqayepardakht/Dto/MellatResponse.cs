using System.Text.Json.Serialization;

namespace PaymentGateway.PaymentApi.PaymentType.Concrete.Aqayepardakht.Dto
{
    public class MellatResponse
    {
        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("responseCode")]
        public string? ResponseCode { get; set; }
    }
}
