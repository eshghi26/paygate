using System.Collections.Concurrent;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PaymentGateway.Domain.Payment.TokenInfos.Dto;
using PaymentGateway.Domain.Payment.Transactions.Dto;
using PaymentGateway.GatewayUi.Models;
using System.Net;

namespace PaymentGateway.GatewayUi.Controllers
{
    public class GatewayController(IHttpClientFactory httpClientFactory,
        IConfiguration config, ILogger<GatewayController> logger) : Controller
    {
        public async Task<IActionResult> Payment(string? id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return View("NotFoundContent");
            }

            try
            {
                var baseUrl = config["Settings:PaymentBaseUrl"];
                var paymentSecretKey = config["Settings:PaymentSecretKey"];

                var client = httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Add("SecretKey", paymentSecretKey);

                var apiAddress = $"{baseUrl}/api/v1/Payment/GetTokenInfo/{id}";
                var response = await client.GetAsync(apiAddress);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    return View("NotFoundContent");
                }

                var result = await response.Content.ReadFromJsonAsync<GetTokenInfoResponse>();
                if (result == null)
                {
                    return View("NotFoundContent");
                }

                var model = new PaymentViewModel
                {
                    Amount = result.Amount,
                    Cards = result.Cards?.Select(c => new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = GetCardFormat(c.Pan, true)
                    }).ToList(),
                    CaptchaAddress = result.CaptchaAddress,
                    Token = id
                };

                var dis = result.ExpireDate - DateTime.Now;
                model.ExpireTime = dis.TotalMilliseconds;

                if (result.Cards != null && result.Cards.Any())
                {
                    if (_cardDic.ContainsKey(id))
                    {
                        _cardDic.TryRemove(id.Trim(), out _);
                    }

                    _cardDic.TryAdd(id, new UserCardsModel
                    {
                        ExpireDate = result.ExpireDate,
                        Cards = result.Cards.Select(c => new CardViewModel
                        {
                            Id = c.Id,
                            Pan = c.Pan,
                            Cvv2 = c.Cvv2,
                            ExpYear = c.ExpYear,
                            ExpMonth = c.ExpMonth
                        }).ToArray()
                    });
                }

                return View(model);
            }
            catch (Exception exp)
            {
                logger.LogError(exp, exp.Message);

                return View("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> StartTransaction(PaymentViewModel data)
        {
            try
            {
                #region Validation
                if (string.IsNullOrWhiteSpace(data.Token))
                {
                    return StatusCode(500, new { message = "توکن ارسالی معتبر نمی باشد. لطفا تغییری در آدرس صفحه پرداخت ایجاد نکنید" });
                }

                if (string.IsNullOrWhiteSpace(data.Pan))
                {
                    return StatusCode(500, new { message = "لطفا شماره کارت را وارد نمایید" });
                }

                var pan = data.Pan.Trim().Replace(" ", "");

                if (pan.Length != 16)
                {
                    return StatusCode(500, new { message = "شماره کارت 16 رقمی را بصورت صحیح وارد نمایید" });
                }
                #endregion

                #region Call Payment Api
                var startTrans = new StartTransactionDto
                {
                    Token = data.Token,
                    Captcha = data.CaptchaCode,
                    SavePan = data.SavePan,
                    Pan = pan,
                    Cvv2 = data.Cvv2,
                    ExpMonth = data.ExpMonth,
                    ExpYear = data.ExpYear
                };

                var baseUrl = config["Settings:PaymentBaseUrl"];
                var paymentSecretKey = config["Settings:PaymentSecretKey"];

                var client = httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Add("SecretKey", paymentSecretKey);

                var apiAddress = $"{baseUrl}/api/v1/Payment/StartTransaction";
                var response = await client.PostAsJsonAsync(apiAddress, startTrans);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    if (response.StatusCode == HttpStatusCode.ExpectationFailed)
                    {
                        var errorMessage = await response.Content.ReadAsStringAsync();
                        return StatusCode(500, new { message = errorMessage });
                    }

                    return StatusCode(500, new { message = "خطا در پاسخ وب سرور بانک. لطفا مجددا تلاش بفرمایید" });
                }

                var result = await response.Content.ReadAsStringAsync();
                if (result != "true")
                {
                    return StatusCode(500, new { message = "عملیات نا موفق. لطفا مجددا تلاش بفرمایید" });
                }

                return PartialView("PaymentOtp");
                #endregion
            }
            catch (Exception exp)
            {
                logger.LogError(exp, exp.Message);

                return StatusCode(500, new { message = "خطای سیستمی لطفا کمی بعد مجددا تلاش کنید" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> FinishTransaction(FinishTransactionModel data)
        {
            try
            {
                #region Validation
                if (string.IsNullOrWhiteSpace(data.Token))
                {
                    return StatusCode(500, new { message = "توکن ارسالی معتبر نمی باشد. لطفا تغییری در آدرس صفحه پرداخت ایجاد نکنید" });
                }

                if (string.IsNullOrWhiteSpace(data.Pin))
                {
                    return StatusCode(500, new { message = "لطفا رمز پویا را وارد نمایید" });
                }

                if (data.Pin.Trim().Length < 5)
                {
                    return StatusCode(500, new { message = "رمز پویا حداقل بایید 5 رقم باشد" });
                }
                #endregion

                #region Call Payment Api
                var finishTrans = new FinishTransactionDto
                {
                    Token = data.Token,
                    Pin = data.Pin
                };

                var baseUrl = config["Settings:PaymentBaseUrl"];
                var paymentSecretKey = config["Settings:PaymentSecretKey"];

                var client = httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Add("SecretKey", paymentSecretKey);

                var apiAddress = $"{baseUrl}/api/v1/Payment/FinishTransaction";
                var response = await client.PostAsJsonAsync(apiAddress, finishTrans);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    if (response.StatusCode == HttpStatusCode.ExpectationFailed)
                    {
                        var errorMessage = await response.Content.ReadAsStringAsync();
                        return StatusCode(500, new { message = errorMessage });
                    }

                    return StatusCode(500, new { message = "خطا در پاسخ وب سرور بانک. لطفا مجددا تلاش بفرمایید" });
                }

                var result = await response.Content.ReadAsStringAsync();
                if (result != "true")
                {
                    return StatusCode(500, new { message = "عملیات نا موفق. لطفا مجددا تلاش بفرمایید" });
                }
                #endregion

                var redirectUrl = Url.Action("Result", "Gateway", new { token = data.Token });
                return Json(new { success = true, redirectUrl });
            }
            catch (Exception exp)
            {
                logger.LogError(exp, exp.Message);

                return StatusCode(500, new { message = "خطای سیستمی لطفا کمی بعد مجددا تلاش کنید" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CancelTransaction(string token)
        {
            try
            {
                #region Validation
                if (string.IsNullOrWhiteSpace(token))
                {
                    return StatusCode(500, new { message = "توکن ارسالی معتبر نمی باشد. لطفا تغییری در آدرس صفحه پرداخت ایجاد نکنید" });
                }
                #endregion

                #region Call Payment Api
                var baseUrl = config["Settings:PaymentBaseUrl"];
                var paymentSecretKey = config["Settings:PaymentSecretKey"];

                var client = httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Add("SecretKey", paymentSecretKey);

                var apiAddress = $"{baseUrl}/api/v1/Payment/CancelTransaction/{token}";
                var response = await client.GetAsync(apiAddress);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    if (response.StatusCode == HttpStatusCode.ExpectationFailed)
                    {
                        var errorMessage = await response.Content.ReadAsStringAsync();
                        return StatusCode(500, new { message = errorMessage });
                    }

                    return StatusCode(500, new { message = "خطا در پاسخ وب سرور بانک. لطفا مجددا تلاش بفرمایید" });
                }

                var result = await response.Content.ReadAsStringAsync();
                if (result != "true")
                {
                    return StatusCode(500, new { message = "عملیات نا موفق. لطفا مجددا تلاش بفرمایید" });
                }
                #endregion

                var redirectUrl = Url.Action("Result", "Gateway", new { token });
                return Json(new { success = true, redirectUrl });
            }
            catch (Exception exp)
            {
                logger.LogError(exp, exp.Message);

                return StatusCode(500, new { message = "خطای سیستمی لطفا کمی بعد مجددا تلاش کنید" });
            }
        }

        public async Task<IActionResult> Result(string? token)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(token))
                {
                    return View("NotFoundContent");
                }

                var baseUrl = config["Settings:PaymentBaseUrl"];
                var paymentSecretKey = config["Settings:PaymentSecretKey"];

                var client = httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Add("SecretKey", paymentSecretKey);

                var apiAddress = $"{baseUrl}/api/v1/Payment/GetPaymentInfo/{token}";
                var response = await client.GetAsync(apiAddress);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    return View("NotFoundContent");
                }

                var result = await response.Content.ReadFromJsonAsync<GetPaymentInfoResponse>();
                if (result == null)
                {
                    return View("NotFoundContent");
                }

                var model = new PaymentResultModel
                {
                    Token = result.Token,
                    Amount = result.Amount,
                    FinalAmount = result.FinalAmount,
                    InvoiceNumber = result.InvoiceNumber,
                    TrackingNumber = result.TrackingNumber,
                    Status = result.Status,
                    CallbackUrl = result.CallbackUrl
                };

                PurgeCards();

                return View("PaymentResult", model);
            }
            catch (Exception exp)
            {
                logger.LogError(exp, exp.Message);

                return View("NotFoundContent");
            }
        }

        [HttpPost]
        public IActionResult CardListChange(string token, string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(token)
                    || !long.TryParse(id, out var cardId)
                    || !_cardDic.TryGetValue(token, out var tokenCard))
                {
                    return Json(null);
                }

                var card = tokenCard.Cards?.FirstOrDefault(c => c.Id == cardId);
                if (card == null)
                {
                    return Json(null);
                }

                var response = new CardViewModel
                {
                    Id = card.Id,
                    Pan = GetCardFormat(card.Pan),
                    Cvv2 = card.Cvv2,
                    ExpMonth = card.ExpMonth,
                    ExpYear = card.ExpYear,
                };

                return Json(response);
            }
            catch (Exception exp)
            {
                logger.LogError(exp, exp.Message);

                return Json(null);
            }
        }
        
        private static ConcurrentDictionary<string, UserCardsModel> _cardDic = new();
        private string? GetCardFormat(string? cardNumber, bool isMusked = false)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(cardNumber))
                    return null;

                if (isMusked && cardNumber.Length == 16)
                {
                    cardNumber = cardNumber.Substring(0, 6) + "******" + cardNumber.Substring(12, 4);
                }

                var retStr = string.Empty;

                for (var i = 1; i <= cardNumber.Length; i++)
                {
                    retStr += i % 4 == 0 ? cardNumber[i - 1] + " " : cardNumber[i - 1];
                }

                return retStr;
            }
            catch (Exception exp)
            {
                logger.LogError(exp, exp.Message);

                return null;
            }
        }

        private void PurgeCards()
        {
            try
            {
                var expiredCards = _cardDic
                    .Where(c => c.Value.ExpireDate < DateTime.Now)
                    .Select(c => c.Key)
                    .ToArray();

                foreach (var token in expiredCards)
                {
                    _cardDic.TryRemove(token, out _);
                }
            }
            catch (Exception exp)
            {
                logger.LogError(exp, exp.Message);
            }
        }
    }
}
