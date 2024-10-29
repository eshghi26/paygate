using Common.Helper.Enum;
using Microsoft.AspNetCore.Mvc;

namespace PaymentGateway.GatewayUi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class PaymentController(IHttpClientFactory httpClient, 
        IConfiguration config, ILogger<GatewayController> logger) : ControllerBase
    {
        [HttpPost]
        [Route("Create")]
        public async Task<ActionResult<object?>> Create(object data)
        {
            try
            {
                var paymentBaseUrl = config["Settings:PaymentBaseUrl"];
                var paymentSecretKey = config["Settings:PaymentSecretKey"];
                var request = data.ToString();

                if (request == null)
                {
                    return new
                    {
                        Status = "error",
                        Code = ((short)PaymentResponseErrorType.RequiredKey).ToString()
                    };
                }

                var client = httpClient.CreateClient();
                client.DefaultRequestHeaders.Add("SecretKey", paymentSecretKey);
                
                var content = new StringContent(request, System.Text.Encoding.UTF8, "application/json");

                var response = await client.PostAsync($"{paymentBaseUrl}/api/v1/Payment/Create", content);

                if (!response.IsSuccessStatusCode)
                {
                    return new
                    {
                        Status = "error",
                        Code = ((short)PaymentResponseErrorType.Unknown).ToString()
                    };
                }

                var responseData = await response.Content.ReadAsStringAsync();

                var jsonObject = System.Text.Json.JsonSerializer.Deserialize<object?>(responseData);

                return jsonObject;
            }
            catch (Exception exp)
            {
                logger.LogError(exp, exp.Message);

                return new
                {
                    Status = "error",
                    Code = ((short)PaymentResponseErrorType.Unknown).ToString()
                };
            }
        }

        [HttpPost]
        [Route("Verify")]
        public async Task<ActionResult<object?>> Verify(object data)
        {
            try
            {
                var paymentBaseUrl = config["Settings:PaymentBaseUrl"];
                var paymentSecretKey = config["Settings:PaymentSecretKey"];
                var request = data.ToString();

                if (request == null)
                {
                    return new
                    {
                        Status = "error",
                        Code = ((short)PaymentResponseErrorType.RequiredKey).ToString()
                    };
                }

                var client = httpClient.CreateClient();
                client.DefaultRequestHeaders.Add("SecretKey", paymentSecretKey);

                var content = new StringContent(request, System.Text.Encoding.UTF8, "application/json");

                var response = await client.PostAsync($"{paymentBaseUrl}/api/v1/Payment/Verify", content);

                if (!response.IsSuccessStatusCode)
                {
                    return new
                    {
                        Status = "error",
                        Code = ((short)PaymentResponseErrorType.Unknown).ToString()
                    };
                }

                var responseData = await response.Content.ReadAsStringAsync();

                var jsonObject = System.Text.Json.JsonSerializer.Deserialize<object?>(responseData);

                return jsonObject;
            }
            catch (Exception exp)
            {
                logger.LogError(exp, exp.Message);

                return new
                {
                    Status = "error",
                    Code = ((short)PaymentResponseErrorType.Unknown).ToString()
                };
            }
        }
    }
}