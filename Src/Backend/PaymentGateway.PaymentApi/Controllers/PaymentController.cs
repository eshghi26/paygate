#region Using
using AutoMapper;
using Common.Helper.Enum;
using Common.Helper.Extension;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using PaymentGateway.Application.Payment.Gateways.Queries;
using PaymentGateway.Application.Payment.Merchants.Queries;
using PaymentGateway.Application.Payment.TokenInfos.Commands;
using PaymentGateway.Application.Payment.TokenInfos.Queries;
using PaymentGateway.Application.Payment.Transactions.Queries;
using PaymentGateway.Application.Payment.UserCards.Queries;
using PaymentGateway.Domain.Base;
using PaymentGateway.Domain.Payment.TokenInfos.Dto;
using PaymentGateway.Domain.Payment.TokenInfos.Dto.Object;
using PaymentGateway.Domain.Payment.Transactions.Dto;
using PaymentGateway.Domain.Payment.UserCards;
using PaymentGateway.PaymentApi.Helper;
using PaymentGateway.PaymentApi.PaymentType.Concrete.Aqayepardakht;
using PaymentGateway.PaymentApi.PaymentType.Factory;
#endregion

namespace PaymentGateway.PaymentApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class PaymentController(IMediator mediator, IMapper mapper,
        IConfiguration config, PaymentFactory paymentFactory,
        ILogger<PaymentController> logger) : BaseController
    {
        #region Create Token
        [HttpPost]
        [Route("Create")]
        [SecretKeyCheck]
        public async Task<ActionResult<BaseResponse>> Create(CreateTokenRequestDto data)
        {
            try
            {
                #region Validation
                if (string.IsNullOrWhiteSpace(data.Key?.Trim()))
                {
                    return new BaseResponse(PaymentResponseErrorType.RequiredKey);
                }

                if (data.Amount <= 0)
                {
                    return new BaseResponse(PaymentResponseErrorType.RequiredAmount);
                }

                if (string.IsNullOrWhiteSpace(data.Callback?.Trim()))
                {
                    return new BaseResponse(PaymentResponseErrorType.RequiredCallback);
                }

                var gateway = await mediator.Send(new GetGatewayByKeyQuery { Key = data.Key });

                if (gateway == null)
                {
                    return new BaseResponse(PaymentResponseErrorType.NotFindGateway);
                }

                if (gateway.Status != (short)GatewayStatus.Active)
                {
                    return new BaseResponse(PaymentResponseErrorType.DeactivateGateway);
                }

                if (data.Amount < gateway.MinAmount || data.Amount > gateway.MaxAmount)
                {
                    return new BaseResponse(PaymentResponseErrorType.OutOffRangeAmount);
                }

                var merchant = await mediator.Send(new GetMerchantByIdQuery { Id = gateway.MerchantId });
                if (merchant == null || !merchant.IsActive || merchant.IsDeleted)
                {
                    return new BaseResponse(PaymentResponseErrorType.DeactivateMerchant);
                }
                #endregion

                #region Create Token
                var dt = DateTime.Now;
                var token = Guid.NewGuid();
                var tokenCommand = new AddTokenCommand
                {
                    Token = token,
                    Amount = data.Amount,
                    MerchantId = gateway.MerchantId,
                    GatewayId = gateway.Id,
                    CallBackUrl = data.Callback,
                    CreateOn = dt,
                    ExpireDate = dt.AddSeconds(600),
                    MerchantUserId = data.UserId,
                    InvoiceNumber = data.InvoiceNumber
                };
                var tokenId = await mediator.Send(tokenCommand);

                if (tokenId > 0)
                {
                    return new BaseResponse(token);
                }
                #endregion

                return new BaseResponse(PaymentResponseErrorType.Unknown);
            }
            catch (Exception exp)
            {
                logger.LogError(exp, exp.Message);
                return new BaseResponse(PaymentResponseErrorType.Unknown);
            }
        }
        #endregion

        #region Get Token Info
        [HttpGet]
        [Route("GetTokenInfo/{token}")]
        [SecretKeyCheck]
        public async Task<ActionResult<GetTokenInfoResponse>>
            GetTokenInfo(string? token)
        {
            try
            {
                #region Validation
                if (string.IsNullOrWhiteSpace(token?.Trim()))
                {
                    return ValidationError(new[,] { { "token", "Token is required" } });
                }

                var tokenModel = await mediator.Send(new GetTokenInfoByTokenQuery
                {
                    Token = token.Trim()
                });

                if (tokenModel == null)
                {
                    return ValidationError(new[,] { { "token", "Invalid token" } });
                }

                if (tokenModel.ExpireDate < DateTime.Now)
                {
                    return ValidationError(new[,] { { "token", "Token expired" } });
                }

                var gateway = await mediator.Send(new GetGatewayByIdQuery
                {
                    Id = tokenModel.GatewayId
                });

                if (gateway == null || gateway.Status != (short)GatewayStatus.Active)
                {
                    return ValidationError(new[,] { { "gateway", "Deactivate gateway" } });
                }

                var baseAddress = config["Settings:BaseUrl"];
                #endregion

                #region Check Transaction Number
                if (string.IsNullOrWhiteSpace(tokenModel.TrCode))
                {
                    var payment = paymentFactory.CreatePaymentFactory(gateway.Type, gateway.ModuleType);
                    var trCode = await payment.GetTransactionCode(gateway, tokenModel);

                    if (!trCode.IsSuccess || trCode.Data == null)
                    {
                        return ValidationError(new[,] { { "TrCode", "Can not get transaction code" } });
                    }

                    var editToken = mapper.Map<EditTokenCommand>(trCode.Data);
                    var updated = await mediator.Send(editToken);

                    if (!updated)
                    {
                        return ValidationError(new[,] { { "TokenInfo", "Can not update token info" } });
                    }
                }
                #endregion

                #region Get User Cards
                List<UserCard>? cards = null;

                if (!string.IsNullOrWhiteSpace(tokenModel.MerchantUserId?.Trim()))
                {
                    cards = await mediator.Send(new GetUserCardsByUserIdQuery
                    {
                        UserId = tokenModel.MerchantUserId.Trim(),
                        MerchantId = tokenModel.MerchantId
                    });
                }
                #endregion

                #region Generate response
                return new GetTokenInfoResponse
                {
                    Amount = tokenModel.Amount,
                    ExpireDate = tokenModel.ExpireDate,
                    CaptchaAddress = $"{baseAddress}/api/v1/Resource/GetResource/{tokenModel.Captcha}.jpg",
                    Cards = cards != null && cards.Any() ? cards : null
                };
                #endregion
            }
            catch (Exception exp)
            {
                logger.LogError(exp, exp.Message);
                return FailedOperation("Exp");
            }
        }
        #endregion

        #region Get Payment Info
        [HttpGet]
        [Route("GetPaymentInfo/{token}")]
        [SecretKeyCheck]
        public async Task<ActionResult<GetPaymentInfoResponse>>
            GetPaymentInfo(string? token)
        {
            try
            {
                #region Validation
                if (string.IsNullOrWhiteSpace(token?.Trim()))
                {
                    return FailedOperation("توکن اجباری می باشد");
                }

                var tokenModel = await mediator.Send(new GetTokenInfoByTokenQuery { Token = token.Trim()});

                if (tokenModel == null)
                {
                    return FailedOperation("توکن ارسالی معتبر نمی باشد");
                }

                var transaction = await mediator.Send(new GetTransactionByTokenQuery
                {
                    Token = tokenModel.Token
                });

                if (transaction == null)
                {
                    return FailedOperation("تراکنشی مربوط به این توکن یافت نشد");
                }
                
                if (transaction.Status != (short)TransactionStatusType.WaitForVerify 
                    && transaction.Status != (short)TransactionStatusType.Canceled)
                {
                    return FailedOperation("این تراکنش در وضعیتی نمی باشد که بتوان استعلام گرفت");
                }
                #endregion

                var transactionStatus =
                    transaction.Status == (short)TransactionStatusType.WaitForVerify
                        ? (short)1
                        : (short)0;

                return new GetPaymentInfoResponse
                {
                    Token = transaction.Token.ToString(),
                    Amount = transaction.Amount,
                    FinalAmount = transaction.FinalAmount,
                    TrackingNumber = transaction.TrackingNumber,
                    InvoiceNumber = transaction.InvoiceNumber,
                    Status = transactionStatus,
                    CallbackUrl = tokenModel.CallBackUrl
                };
            }
            catch (Exception exp)
            {
                logger.LogError(exp, exp.Message);
                return FailedOperation("Exp");
            }
        }
        #endregion

        #region StartTransaction (Get OTP)
        [HttpPost]
        [Route("StartTransaction")]
        [SecretKeyCheck]
        public async Task<ActionResult<bool>>
            StartTransaction(StartTransactionDto data)
        {
            try
            {
                #region Validation
                var tokenModel = await mediator.Send(new GetTokenInfoByTokenQuery
                {
                    Token = data.Token
                });

                if (tokenModel == null)
                {
                    return FailedOperation("توکن ارسالی معتبر نمی باشد. لطفا تغییری در آدرس صفحه پرداخت ایجاد نکنید");
                }

                if (tokenModel.ExpireDate < DateTime.Now)
                {
                    return FailedOperation("فرصت شما برای انجام این تراکنش به پایان رسیده. لطفا مجددا از طریق سایت پذیرنده اقدام به انجام تراکنش نمایید");
                }

                var gateway = await mediator.Send(new GetGatewayByIdQuery
                {
                    Id = tokenModel.GatewayId
                });

                if (gateway == null || gateway.Status != (short)GatewayStatus.Active)
                {
                    return FailedOperation("درگاه شما غیر فعال می باشد. لطفا برای فعال نمودن درگاه با مدیر سایت در ارتباط باشید");
                }
                #endregion

                var payment = paymentFactory.CreatePaymentFactory(gateway.Type, gateway.ModuleType);
                var startTransaction = await payment.StartTransaction(tokenModel, data);

                if (startTransaction.IsSuccess)
                {
                    var card = new CardObject
                    {
                        Pan = data.Pan,
                        Cvv2 = data.Cvv2,
                        ExpYear = data.ExpYear,
                        ExpMonth = data.ExpMonth,
                        SavePan = data.SavePan,
                        Captcha = data.Captcha
                    };

                    tokenModel.Obj = card.ToJson();
                    var command = mapper.Map<EditTokenCommand>(tokenModel);
                    var updated = await mediator.Send(command);

                    if (!updated)
                    {
                        logger.LogWarning($"Can not update card object info{Environment.NewLine}{tokenModel.Obj}");
                    }

                    return true;
                }

                return FailedOperation(startTransaction.ErrorMessage ?? "خطا در ارسال رمز پویا");
            }
            catch (Exception exp)
            {
                logger.LogError(exp, exp.Message);
                return FailedOperation("در ارسال رمز پویا خطایی رخ داده لطفا تا دقایقی دیگر مجددا تلاش بفرمایید");
            }
        }
        #endregion

        #region FinishTransaction
        [HttpPost]
        [Route("FinishTransaction")]
        [SecretKeyCheck]
        public async Task<ActionResult<bool>>
            FinishTransaction(FinishTransactionDto data)
        {
            try
            {
                #region Validation
                var tokenModel = await mediator.Send(new GetTokenInfoByTokenQuery
                {
                    Token = data.Token
                });

                if (tokenModel == null)
                {
                    return FailedOperation("توکن ارسالی معتبر نمی باشد. لطفا تغییری در آدرس صفحه پرداخت ایجاد نکنید");
                }

                if (tokenModel.ExpireDate < DateTime.Now)
                {
                    return FailedOperation("فرصت شما برای انجام این تراکنش به پایان رسیده. لطفا مجددا از طریق سایت پذیرنده اقدام به انجام تراکنش نمایید");
                }

                var gateway = await mediator.Send(new GetGatewayByIdQuery
                {
                    Id = tokenModel.GatewayId
                });

                if (gateway == null || gateway.Status != (short)GatewayStatus.Active)
                {
                    return FailedOperation("درگاه شما غیر فعال می باشد. لطفا برای فعال نمودن درگاه با مدیر سایت در ارتباط باشید");
                }
                #endregion

                var payment = paymentFactory.CreatePaymentFactory(gateway.Type, gateway.ModuleType);
                var finishTransaction = await payment.FinishTransaction(tokenModel, data.Pin);

                if (finishTransaction.IsSuccess)
                {
                    return true;
                }

                return FailedOperation(finishTransaction.ErrorMessage ?? "خطا در ارسال رمز پویا");
            }
            catch (Exception exp)
            {
                logger.LogError(exp, exp.Message);
                return FailedOperation("در عملیات پرداخت خطایی رخ داده لطفا تا دقایقی دیگر مجددا تلاش بفرمایید");
            }
        }
        #endregion

        #region Verify Transaction
        [HttpPost]
        [Route("Verify")]
        [SecretKeyCheck]
        public async Task<ActionResult<BaseResponse>> 
            Verify(VerifyTransactionDto data)
        {
            try
            {
                #region Validation
                if (string.IsNullOrWhiteSpace(data.Key?.Trim()))
                {
                    return new BaseResponse(PaymentResponseErrorType.RequiredKey);
                }

                if (data.Amount <= 0)
                {
                    return new BaseResponse(PaymentResponseErrorType.RequiredAmount);
                }

                if (string.IsNullOrWhiteSpace(data.Token?.Trim()))
                {
                    return new BaseResponse(PaymentResponseErrorType.RequiredToken);
                }

                var gateway = await mediator.Send(new GetGatewayByKeyQuery { Key = data.Key });

                if (gateway == null)
                {
                    return new BaseResponse(PaymentResponseErrorType.NotFindGateway);
                }

                if (gateway.Status != (short)GatewayStatus.Active)
                {
                    return new BaseResponse(PaymentResponseErrorType.DeactivateGateway);
                }
                
                var merchant = await mediator.Send(new GetMerchantByIdQuery { Id = gateway.MerchantId });
                if (merchant == null || !merchant.IsActive || merchant.IsDeleted)
                {
                    return new BaseResponse(PaymentResponseErrorType.DeactivateMerchant);
                }

                var token = Guid.Parse(data.Token.Trim());
                var transaction = await mediator.Send(new GetTransactionByTokenQuery{ Token = token});

                if (transaction == null)
                {
                    return new BaseResponse(PaymentResponseErrorType.NotFindTransaction);
                }

                if (transaction.Status == (short)TransactionStatusType.VerifiedAndDone)
                {
                    return new BaseResponse(PaymentResponseErrorType.TransactionHasAlreadyBeenVerified);
                }

                if (transaction.Status != (short)TransactionStatusType.WaitForVerify)
                {
                    return new BaseResponse(PaymentResponseErrorType.TransactionStatusIsNotVerifiable);
                }

                if (transaction.Amount != data.Amount)
                {
                    return new BaseResponse(PaymentResponseErrorType.InvalidAmount);
                }
                #endregion

                var payment = paymentFactory.CreatePaymentFactory(gateway.Type, gateway.ModuleType);
                var verifyTransaction = await payment.VerifyTransaction(gateway, transaction);

                if (verifyTransaction.IsSuccess)
                {
                    return new BaseResponse("1");
                }

                return new BaseResponse(PaymentResponseErrorType.Unknown);
            }
            catch (Exception exp)
            {
                logger.LogError(exp, exp.Message);
                return new BaseResponse(PaymentResponseErrorType.Unknown);
            }
        }
        #endregion

        #region CancelTransaction
        [HttpGet]
        [Route("CancelTransaction/{token}")]
        [SecretKeyCheck]
        public async Task<ActionResult<bool>>
        CancelTransaction(string token)
        {
            try
            {
                #region Validation
                if (string.IsNullOrWhiteSpace(token.Trim()))
                {
                    return FailedOperation("توکن ارسالی معتبر نمی باشد. لطفا تغییری در آدرس صفحه پرداخت ایجاد نکنید");
                }

                var tokenModel = await mediator.Send(new GetTokenInfoByTokenQuery
                {
                    Token = token.Trim()
                });

                if (tokenModel == null)
                {
                    return FailedOperation("توکن ارسالی معتبر نمی باشد. لطفا تغییری در آدرس صفحه پرداخت ایجاد نکنید");
                }

                if (tokenModel.ExpireDate < DateTime.Now)
                {
                    return FailedOperation("فرصت شما برای انجام این تراکنش به پایان رسیده. لطفا مجددا از طریق سایت پذیرنده اقدام به انجام تراکنش نمایید");
                }

                var gateway = await mediator.Send(new GetGatewayByIdQuery
                {
                    Id = tokenModel.GatewayId
                });

                if (gateway == null || gateway.Status != (short)GatewayStatus.Active)
                {
                    return FailedOperation("درگاه شما غیر فعال می باشد. لطفا برای فعال نمودن درگاه با مدیر سایت در ارتباط باشید");
                }
                #endregion

                var payment = paymentFactory.CreatePaymentFactory(gateway.Type, gateway.ModuleType);
                var cancelTransaction = await payment.CancelTransaction(tokenModel);

                if (cancelTransaction.IsSuccess)
                {
                    return true;
                }

                return FailedOperation(cancelTransaction.ErrorMessage ?? "خطا در لغو تراکنش");
            }
            catch (Exception exp)
            {
                logger.LogError(exp, exp.Message);
                return FailedOperation("در عملیات پرداخت خطایی رخ داده لطفا تا دقایقی دیگر مجددا تلاش بفرمایید");
            }
        }
        #endregion
    }
}