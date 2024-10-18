using PaymentGateway.Domain.Payment.Gateways;
using PaymentGateway.PaymentApi.PaymentType.Factory;
using System.Text.Json.Serialization;
using HtmlAgilityPack;
using MediatR;
using PaymentGateway.Application.Payment.Resources.Commands;
using Common.Helper.Helper;
using PaymentGateway.Domain.Payment.TokenInfos;
using PaymentGateway.Domain.Payment.TokenInfos.Dto;
using System.Text.RegularExpressions;

namespace PaymentGateway.PaymentApi.PaymentType.Concrete
{
    public class ConcreteAqayepardakht(IHttpClientFactory httpClientFactory,
        IMediator mediator, IConfiguration config) : IPayment
    {
        public async Task<(bool IsSuccess, GetTransactionCodeModel? Data, string? ErrorMessage)> 
            GetTransactionCode(Gateway gateway, decimal amount)
        {
            var client = httpClientFactory.CreateClient();
            
            var paymentAddress = string.Empty;
            var retModel = new GetTransactionCodeModel();

            #region Get Payment Address
            try
            {
                var baseUrl = config["Settings:BaseUrl"];
                var response = await client.PostAsJsonAsync("https://panel.aqayepardakht.ir/api/v2/create",
                    new
                    {
                        pin = gateway.Pin,
                        amount = amount,
                        callback = $"{baseUrl}/callback"
                    });

                var result = await response.Content.ReadFromJsonAsync<TokenResponse>();
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    retModel.TransactionCode = result?.TransactionId;
                    paymentAddress = $"https://panel.aqayepardakht.ir/startpay/{result?.TransactionId}";
                }
            }
            catch (Exception exp)
            {
                return (false, null, "Problem in Get Aqayepardakht token");
            }
            #endregion

            #region Go to Psp ipg
            if (string.IsNullOrWhiteSpace(paymentAddress))
            {
                return (false, null, "Can not get payment address");
            }

            try
            {
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");
                var res = await client.GetAsync(paymentAddress);
                res.EnsureSuccessStatusCode();

                string responseBody = await res.Content.ReadAsStringAsync();

                var doc = new HtmlDocument();
                doc.LoadHtml(responseBody);

                var captcha = doc.DocumentNode.SelectSingleNode("//img[@id='captcha-img']");

                if (captcha != null)
                {
                    var src = captcha.GetAttributeValue("src", "");
                    var refId = GetRefId(src);

                    if (!string.IsNullOrWhiteSpace(src) && !string.IsNullOrWhiteSpace(refId))
                    {
                        retModel.ReserveNumber = refId;

                        var captchaAddress = $"https://bpm.shaparak.ir/pgwchannel/{src}";

                        var imageResponse = await client.GetAsync(captchaAddress);
                        if (imageResponse.IsSuccessStatusCode)
                        {
                            var imageBytes = await imageResponse.Content.ReadAsByteArrayAsync();
                            var resource = new AddResourceCommand
                            {
                                Name = SecurityHelper.CreateUniqNumber().ToString(),
                                FileData = imageBytes,
                                CreateOn = DateTime.Now,
                                ExpireDate = DateTime.Now.AddMinutes(30)
                            };
                            var resourceId = await mediator.Send(resource);

                            if (resourceId > 0)
                            {
                                retModel.Captcha = resource.Name;

                                return (true, retModel, null);
                            }
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                return (false, null, "Problem in Get Aqayepardakht psp captcha");
            }
            #endregion

            return (false, null, "Problem in operation");
        }

        private string? GetRefId(string src)
        {
            if (string.IsNullOrWhiteSpace(src.Trim()))
                return null;

            const string pattern = @"RefId=([^\&]+)";

            var match = Regex.Match(src.Trim(), pattern);
            if (!match.Success)
                return null;

            var refId = match.Groups[1].Value;
            return refId.Trim();

        }

        public async Task<(bool IsSuccess, string? ErrorMessage)> 
            StartTransaction(TokenInfo tokenModel, StartTransactionDto data)
        {
            try
            {
                var client = httpClientFactory.CreateClient();
                var postUrl = $"https://bpm.shaparak.ir/pgwchannel/otp-request.mellat?RefId={tokenModel.ReserveNumber}";
                var response = await client.PostAsJsonAsync(postUrl,
                    new
                    {
                        pan = data.Pan,
                        selectedPanIndex = -1,
                        captcha = data.Captcha
                    });

                var result = await response.Content.ReadFromJsonAsync<MellatResponse>();
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    if (result != null && result.Status?.Trim().ToLower() == "OK")
                    {
                        return (true, null);
                    }
                }

                return (false, null);
            }
            catch (Exception exp)
            {
                return (false, null);
            }
        }
    }

    /// <summary>
    /// AghayePardakht Response Model
    /// </summary>
    public class TokenResponse
    {
        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("code")]
        public int? ErrorCode { get; set; }

        [JsonPropertyName("transid")]
        public string? TransactionId { get; set; }
    }

    public class MellatResponse
    {
        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("description")]
        public int? Description { get; set; }
    }
}
