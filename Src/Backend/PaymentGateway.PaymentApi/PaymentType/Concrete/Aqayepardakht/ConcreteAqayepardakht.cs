#region Using
using PaymentGateway.Domain.Payment.Gateways;
using PaymentGateway.PaymentApi.PaymentType.Factory;
using HtmlAgilityPack;
using MediatR;
using PaymentGateway.Application.Payment.Resources.Commands;
using Common.Helper.Helper;
using PaymentGateway.Domain.Payment.TokenInfos;
using PaymentGateway.Domain.Payment.TokenInfos.Dto;
using System.Text.RegularExpressions;
using Common.Helper.Extension;
using PaymentGateway.Domain.Payment.TokenInfos.Dto.Object;
using PaymentGateway.PaymentApi.PaymentType.Concrete.Aqayepardakht.Dto;
using System.Net;
using AutoMapper;
using PaymentGateway.Application.Payment.Transactions.Commands;
using Common.Helper.Enum;
using PaymentGateway.Application.Payment.Transactions.Queries;
using Newtonsoft.Json.Linq;
using PaymentGateway.Application.Payment.UserCards.Commands;
using PaymentGateway.Domain.Payment.Transactions;

#endregion

namespace PaymentGateway.PaymentApi.PaymentType.Concrete.Aqayepardakht
{
    public class ConcreteAqayepardakht(IHttpClientFactory httpClientFactory,
        IMediator mediator, IMapper mapper, IConfiguration config,
        ILogger<ConcreteAqayepardakht> logger) : IPayment
    {
        #region GetTransactionCode
        public async Task<(bool IsSuccess, TokenInfo? Data, string? ErrorMessage)>
            GetTransactionCode(Gateway gateway, TokenInfo tokenModel)
        {
            var client = httpClientFactory.CreateClient();
            var paymentAddress = string.Empty;

            #region Get Payment Address
            try
            {
                var baseUrl = config["Settings:BaseUrl"];
                var response = await client.PostAsJsonAsync("https://panel.aqayepardakht.ir/api/v2/create",
                    new
                    {
                        pin = gateway.Pin,
                        amount = tokenModel.Amount,
                        callback = $"{baseUrl}/api/v1/Result/Aqayepardakht"
                    });

                var result = await response.Content.ReadFromJsonAsync<AqTokenResponse>();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    tokenModel.TrCode = result?.TransactionId;
                    paymentAddress = $"https://panel.aqayepardakht.ir/startpay/{result?.TransactionId}";
                }
            }
            catch (Exception exp)
            {
                logger.LogError(exp, exp.Message);
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

                var returnForm = doc.DocumentNode.SelectSingleNode("//form[@name='returnForm']");
                if (returnForm != null)
                {
                    var returnUrl = returnForm.GetAttributeValue("action", "");
                    tokenModel.ExtraValue1 = returnUrl;
                }

                var order = doc.DocumentNode.SelectSingleNode("//input[@id='SaleOrderId']");
                if (order != null)
                {
                    var orderId = order.GetAttributeValue("value", "");
                    tokenModel.ExtraValue2 = orderId;
                }

                var captcha = doc.DocumentNode.SelectSingleNode("//img[@id='captcha-img']");

                if (captcha != null)
                {
                    var src = captcha.GetAttributeValue("src", "");
                    var refId = GetRefId(src);

                    if (!string.IsNullOrWhiteSpace(src) && !string.IsNullOrWhiteSpace(refId))
                    {
                        tokenModel.ReserveNumber = refId;

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
                                tokenModel.Captcha = resource.Name;

                                var addTransactionCommand = new AddTransactionCommand
                                {
                                    Token = tokenModel.Token,
                                    TrackingNumber = SecurityHelper.CreateUniqNumber().ToString(),
                                    Amount = tokenModel.Amount,
                                    GatewayId = tokenModel.GatewayId,
                                    MerchantId = tokenModel.MerchantId,
                                    OperationType = (short)TransactionOperationType.IpgSkin,
                                    Status = (short)TransactionStatusType.Reserved,
                                    CreateOn = DateTime.Now,
                                    InvoiceNumber = tokenModel.InvoiceNumber,
                                    TransactionCode = tokenModel.TrCode!,
                                    MerchantUserId = tokenModel.MerchantUserId,
                                    WageAmount = 0,
                                    ReserveNumber = tokenModel.ReserveNumber
                                };

                                var transactionId = await mediator.Send(addTransactionCommand);
                                if (transactionId > 0)
                                {
                                    return (true, tokenModel, null);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                logger.LogError(exp, exp.Message);
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
        #endregion

        #region StartTransaction
        public async Task<(bool IsSuccess, string? ErrorMessage)>
            StartTransaction(TokenInfo tokenModel, StartTransactionDto data)
        {
            try
            {
                var transaction = await mediator.Send(new GetTransactionByTokenQuery { Token = tokenModel.Token });

                if (transaction == null)
                {
                    return (false, "تراکنش مورد نظر یافت نشد");
                }

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
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    return (false, "عدم دریافت پاسخ مناسب از سرور بانک");
                }

                if (result == null)
                {
                    return (false, "عدم پاسخگویی سرور بانک. لطفا تا دقایق دیگر مجددا تلاش بفرمایید");
                }

                var status = result.Status?.Trim().ToUpper();

                if (status == "OK")
                {
                    transaction.GetOtpDate = DateTime.Now;
                    transaction.Status = (short)TransactionStatusType.GetOtp;

                    var etc = mapper.Map<EditTransactionCommand>(transaction);

                    var updated = await mediator.Send(etc);

                    if (!updated)
                    {
                        logger.LogWarning($"Can not edit Transaction.{Environment.NewLine}{etc.ToJson()}");
                    }

                    return (true, null);
                }

                return (false, GetErrorMessageFromResult(status));
            }
            catch (Exception exp)
            {
                logger.LogError(exp, exp.Message);
                return (false, "در بر قراری ارتباط با سرور بانک خطایی رخ داده، لطفا تا دقایقی دیگر مجددا تلاش بفرمایید");
            }
        }

        private string GetErrorMessageFromResult(string? status)
        {
            if (string.IsNullOrWhiteSpace(status))
            {
                return "خطا در ارسال درخواست رمز پویا. لطفا تا دقایقی دیگر مجددا تلاش بفرمایید";
            }

            switch (status)
            {
                case "INVALID_CAPTCHA":
                    return "کد تصویر صحیح نمی باشد";

                case "INVALID_PAN":
                    return "شماره کارت وارد شده معتبر نمی باشد";

                default:
                    logger.LogWarning($"Unknown status: {status}");
                    return "خطا در ارسال درخواست رمز پویا. لطفا تا دقایقی دیگر مجددا تلاش بفرمایید";
            }
        }
        #endregion

        #region FinishTransaction
        public async Task<(bool IsSuccess, string? ErrorMessage)>
            FinishTransaction(TokenInfo tokenModel, string pin)
        {
            try
            {
                var transaction = await mediator.Send(new GetTransactionByTokenQuery { Token = tokenModel.Token });

                if (transaction == null)
                {
                    return (false, "تراکنش مورد نظر یافت نشد");
                }

                if (string.IsNullOrWhiteSpace(tokenModel.Obj))
                {
                    return (false, "اطلاعات کارت در سرور یافت نشد");
                }

                var card = tokenModel.Obj.ToModel<CardObject?>();
                if (card == null)
                {
                    return (false, "اطلاعات کارت در سرور یافت نشد");
                }

                var client = httpClientFactory.CreateClient();
                var postUrl = $"https://bpm.shaparak.ir/pgwchannel/sale.mellat?RefId={tokenModel.ReserveNumber}";
                var response = await client.PostAsJsonAsync(postUrl,
                new
                {
                    pan = card.Pan,
                    selectedPanIndex = -1,
                    pin,
                    cvv2 = card.Cvv2,
                    expireMonth = card.ExpMonth,
                    expireYear = card.ExpYear,
                    captcha = card.Captcha,
                    email = "",
                    savePan = false
                });

                var result = await response.Content.ReadFromJsonAsync<MellatResponse>();
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    return (false, "عدم دریافت پاسخ مناسب از سرور بانک");
                }

                if (result == null)
                {
                    return (false, "عدم پاسخگویی سرور بانک. لطفا تا دقایق دیگر مجددا تلاش بفرمایید");
                }

                var status = result.Status?.Trim().ToUpper();

                if (status == "OK")
                {
                    var formData = new Dictionary<string, string>
                    {
                        { "RefId", tokenModel.ReserveNumber! }
                    };
                    var content = new FormUrlEncodedContent(formData);
                    var res = await client.PostAsync("https://bpm.shaparak.ir/pgwchannel/result.mellat", content);

                    if (res.IsSuccessStatusCode)
                    {
                        var resBody = await res.Content.ReadAsStringAsync();

                        var doc = new HtmlDocument();
                        doc.LoadHtml(resBody);

                        var saleReferenceTag = doc.DocumentNode.SelectSingleNode("//input[@id='SaleReferenceId']");
                        if (saleReferenceTag == null)
                        {
                            return (false, "عدم دریافت پاسخ مناسب از سرور بانک");
                        }
                        var saleReferenceId = saleReferenceTag.GetAttributeValue("value", "");

                        var cardHolderTag = doc.DocumentNode.SelectSingleNode("//input[@id='CardHolderInfo']");
                        if (cardHolderTag == null)
                        {
                            return (false, "عدم دریافت پاسخ مناسب از سرور بانک");
                        }
                        var cardHolder = cardHolderTag.GetAttributeValue("value", "");

                        var cardPanTag = doc.DocumentNode.SelectSingleNode("//input[@id='CardHolderPan']");
                        if (cardPanTag == null)
                        {
                            return (false, "عدم دریافت پاسخ مناسب از سرور بانک");
                        }
                        var cardPan = cardPanTag.GetAttributeValue("value", "");

                        var finalAmountTag = doc.DocumentNode.SelectSingleNode("//input[@id='FinalAmount']");
                        if (finalAmountTag == null)
                        {
                            return (false, "عدم دریافت پاسخ مناسب از سرور بانک");
                        }

                        var finalAmount = finalAmountTag.GetAttributeValue("value", "");

                        var frmData = new Dictionary<string, string>
                        {
                            { "RefId", tokenModel.ReserveNumber! },
                            { "ResCode", "0" },
                            { "SaleOrderId", tokenModel.ExtraValue2! },
                            { "SaleReferenceId", saleReferenceId },
                            { "CardHolderInfo", cardHolder },
                            { "CardHolderPan", cardPan },
                            { "FinalAmount", finalAmount },
                        };
                        var agContent = new FormUrlEncodedContent(frmData);
                        var agResponse = await client.PostAsync(tokenModel.ExtraValue1, agContent);

                        if (agResponse.IsSuccessStatusCode)
                        {
                            //var body = await agResponse.Content.ReadAsStringAsync();

                            transaction.Status = (short)TransactionStatusType.WaitForVerify;
                            transaction.FinishTransactionDate = DateTime.Now;
                            transaction.FinalAmount = finalAmount.ToDecimal();

                            var etc = mapper.Map<EditTransactionCommand>(transaction);
                            var updated = await mediator.Send(etc);
                            if (!updated)
                            {
                                logger.LogWarning($"Can not update transaction{Environment.NewLine}{transaction.ToJson()}");
                                return (false, "در حال حاضر امکان انجام این تراکنش وجود ندارد");
                            }

                            if (!string.IsNullOrWhiteSpace(tokenModel.MerchantUserId)
                                  && card.SavePan)
                            {
                                await SaveCard(tokenModel, card);
                            }

                            return (true, null);
                        }
                    }
                }

                return (false, result.Description);
            }
            catch (Exception exp)
            {
                logger.LogError(exp, exp.Message);
                return (false, "در بر قراری ارتباط با سرور بانک خطایی رخ داده، لطفا تا دقایقی دیگر مجددا تلاش بفرمایید");
            }
        }

        private async Task SaveCard(TokenInfo tokenModel, CardObject cardObject)
        {
            try
            {
                var card = new AddUserCardCommand
                {
                    MerchantId = tokenModel.MerchantId,
                    CreateOn = DateTime.Now,
                    UserId = tokenModel.MerchantUserId!,
                    IsDeleted = false,
                    Displayable = true,
                    Pan = cardObject.Pan!,
                    Cvv2 = cardObject.Cvv2!,
                    ExpYear = cardObject.ExpYear!,
                    ExpMonth = cardObject.ExpMonth!
                };

                await mediator.Send(card);
            }
            catch (Exception exp)
            {
                //TODO log
            }
        }
        #endregion

        #region CancelTransaction
        public async Task<(bool IsSuccess, string? ErrorMessage)>
            CancelTransaction(TokenInfo tokenModel)
        {
            try
            {
                //if (string.IsNullOrWhiteSpace(tokenModel.ExtraValue1) ||
                //    string.IsNullOrWhiteSpace(tokenModel.ExtraValue2) ||
                //    string.IsNullOrWhiteSpace(tokenModel.TrCode))
                //{
                //    return (false, "تا دقایقی دیگر این تراکنش بصورت خودکار لغو میشود. شما میتوانید صفحه پرداخت خود را در مرورگر ببندید");
                //}

                //var client = httpClientFactory.CreateClient();
                //var baseUrl = config["Settings:BaseUrl"];
                //var address = $"{baseUrl}/api/v1/Result/Aqayepardakht";

                //var formData = new Dictionary<string, string>
                //{
                //    { "transid", tokenModel.TrCode },
                //    { "tracking_number", tokenModel.ExtraValue2 },
                //    { "bank", "ملت" },
                //    { "status", "0" }
                //};

                //var content = new FormUrlEncodedContent(formData);
                //await client.PostAsync(address, content);
                //return (true, null);
                var transaction = await mediator.Send(new GetTransactionByTokenQuery { Token = tokenModel.Token });

                if (transaction == null)
                {
                    return (false, "تراکنش مورد نظر یافت نشد");
                }

                transaction.Status = (short)TransactionStatusType.Canceled;
                transaction.CancelTransactionDate = DateTime.Now;

                var etc = mapper.Map<EditTransactionCommand>(transaction);
                var updated = await mediator.Send(etc);

                if (!updated)
                {
                    logger.LogWarning($"Can not edit Transaction.{Environment.NewLine}{etc.ToJson()}");
                }

                return (true, null);
            }
            catch (Exception exp)
            {
                logger.LogError(exp, exp.Message);
                return (false, "در بر قراری ارتباط با سرور بانک خطایی رخ داده، لطفا تا دقایقی دیگر مجددا تلاش بفرمایید");
            }
        }

        public async Task<(bool IsSuccess, string? ErrorMessage)>
            CancelTransaction1(TokenInfo tokenModel)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(tokenModel.ExtraValue1) ||
                    string.IsNullOrWhiteSpace(tokenModel.ExtraValue2) ||
                    string.IsNullOrWhiteSpace(tokenModel.TrCode))
                {
                    return (false, "تا دقایقی دیگر این تراکنش بصورت خودکار لغو میشود. شما میتوانید صفحه پرداخت خود را در مرورگر ببندید");
                }

                var formData = new Dictionary<string, string>
                {
                    { "RefId", tokenModel.TrCode },
                    { "ResCode", "17" },
                    { "SaleOrderId", tokenModel.ExtraValue2 }
                };

                var content = new FormUrlEncodedContent(formData);
                var client = httpClientFactory.CreateClient();

                var response = await client.PostAsync(tokenModel.ExtraValue1, content);

                if (response.IsSuccessStatusCode)
                {
                    var resBody = await response.Content.ReadAsStringAsync();

                }

                return (true, null);
            }
            catch (Exception exp)
            {
                return (false, "در بر قراری ارتباط با سرور بانک خطایی رخ داده، لطفا تا دقایقی دیگر مجددا تلاش بفرمایید");
            }
        }
        #endregion

        #region VerifyTransaction
        public async Task<(bool IsSuccess, string? ErrorMessage)>
            VerifyTransaction(Gateway gateway, Transaction transaction)
        {
            try
            {
                var client = httpClientFactory.CreateClient();
                var response = await client.PostAsJsonAsync("https://panel.aqayepardakht.ir/api/v2/verify",
                    new
                    {
                        pin = gateway.Pin,
                        amount = transaction.Amount,
                        transid = transaction.TransactionCode
                    });

                var result = await response.Content.ReadFromJsonAsync<AqVerifyResponse>();
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    return (false, "پاسخ نا مناسب از سرور بانک دریافت شد");
                }

                if (result == null ||
                    string.IsNullOrWhiteSpace(result.Status?.Trim()) ||
                        result.Status.Trim().ToLower() == "error")
                {
                    return (false, result?.Code);
                }

                transaction.Status = (short)TransactionStatusType.VerifiedAndDone;
                transaction.VerifyDate = DateTime.Now;

                var etc = mapper.Map<EditTransactionCommand>(transaction);
                var updated = await mediator.Send(etc);

                if (!updated)
                {
                    logger.LogWarning($"Can not edit Transaction.{Environment.NewLine}{etc.ToJson()}");
                }
                return (true, null);
            }
            catch (Exception exp)
            {
                logger.LogError(exp, exp.Message);
                return (false, "در بر قراری ارتباط با سرور بانک خطایی رخ داده، لطفا تا دقایقی دیگر مجددا تلاش بفرمایید");
            }
        }
        #endregion
    }
}