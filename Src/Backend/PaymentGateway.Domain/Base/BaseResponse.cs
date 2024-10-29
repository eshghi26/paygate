using System.Text.Json.Serialization;
using Common.Helper.Enum;

namespace PaymentGateway.Domain.Base
{
    public class BaseResponse
    {
        public BaseResponse(PaymentResponseErrorType statusCode)
        {
            Status = "error";
            Code = ((short)statusCode).ToString();
        }

        public BaseResponse(Guid token)
        {
            Status = "success";
            Token = token.ToString();
        }

        public BaseResponse(string code)
        {
            Status = "success";
            Code = code;
        }

        public string Status { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Code { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Token { get; set; }
    }
}
