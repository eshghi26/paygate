using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PaymentGateway.BackOffice.Services
{
    public class ApiResult<T>
    {
        public T? Data { get; }
        public ContentStatus Status { get; }
        public string? Message { get; }
        public Dictionary<string, string>? ValidationErrors { get; }

        public ApiResult(string? resultStr, HttpStatusCode statusCode)
        {
            #region Fill Status
            if (statusCode is HttpStatusCode.OK or HttpStatusCode.Accepted)
            {
                Status = ContentStatus.Ok;
            }
            else if (statusCode == HttpStatusCode.UnprocessableEntity)
            {
                Status = ContentStatus.ValidationError;
            }
            else
            {
                Status = ContentStatus.OtherError;
            }
            #endregion

            #region Fill Data
            if (!string.IsNullOrWhiteSpace(resultStr) && Status == ContentStatus.Ok)
            {
                var type = typeof(T);

                if (IsClassType<T>())
                {
                    Data = JsonConvert.DeserializeObject<T>(resultStr);
                }
                else
                {
                    Data = (T)Convert.ChangeType(resultStr, type);
                }
            }
            #endregion

            #region Fill Message
            if (Status == ContentStatus.OtherError)
            {
                Message = resultStr;
            }
            #endregion

            #region Fill ValidationErrors
            if (Status == ContentStatus.ValidationError && !string.IsNullOrWhiteSpace(resultStr))
            {
                var jObject = JObject.Parse(resultStr);
                
                var errors = jObject["message"]?["errors"]?.ToString();

                if (!string.IsNullOrWhiteSpace(errors))
                {
                    var validations = JsonConvert.DeserializeObject<Dictionary<string, string>>(errors);

                    if (validations != null && validations.Any())
                    {
                        ValidationErrors = validations;
                    }
                }
            }
            #endregion
        }

        private bool IsClassType<TE>()
        {
            Type type = typeof(TE);

            return type.IsClass && type != typeof(string);
        }
    }

    public enum ContentStatus
    {
        Ok,
        ValidationError,
        OtherError
    }
}
