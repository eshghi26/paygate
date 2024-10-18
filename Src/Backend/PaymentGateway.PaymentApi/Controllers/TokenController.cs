#region Using
using AutoMapper;
using Common.Helper.Enum;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using PaymentGateway.Application.Payment.Gateways.Queries;
using PaymentGateway.Application.Payment.Merchants.Queries;
using PaymentGateway.Application.Payment.TokenInfos.Commands;
using PaymentGateway.Application.Payment.TokenInfos.Queries;
using PaymentGateway.Application.Payment.UserCards.Queries;
using PaymentGateway.Domain.Base;
using PaymentGateway.Domain.Payment.Gateways;
using PaymentGateway.Domain.Payment.TokenInfos.Dto;
using PaymentGateway.Domain.Payment.UserCards;
using PaymentGateway.PaymentApi.Helper;
using PaymentGateway.PaymentApi.PaymentType.Factory;
#endregion

namespace PaymentGateway.PaymentApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class TokenController (IMediator mediator, IMapper mapper,
        IConfiguration config, PaymentFactory paymentFactory) : BaseController
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
                    MerchantUserId = data.UserId
                };
                var tokenId = await mediator.Send(tokenCommand);

                if (tokenId > 0)
                {
                    return new BaseResponse(token.ToString());
                }
                #endregion

                return new BaseResponse(PaymentResponseErrorType.Unknown);
            }
            catch (Exception exp)
            {
                return new BaseResponse(PaymentResponseErrorType.Unknown);
            }
        }
        #endregion

        #region Get Token
        [HttpGet]
        [Route("GetTokenInfo/{token}")]
        [SecretKeyCheck]
        public async Task<ActionResult<GetTokenInfoResponse>> GetTokenInfo(string? token)
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
                    var trCode = await payment.GetTransactionCode(gateway, tokenModel.Amount);

                    if (!trCode.IsSuccess || trCode.Data == null)
                    {
                        return ValidationError(new[,] { { "TrCode", "Can not get transaction code" } });
                    }

                    tokenModel.TrCode = trCode.Data.TransactionCode;
                    tokenModel.ReserveNumber = trCode.Data.ReserveNumber;
                    tokenModel.Captcha = trCode.Data.Captcha;


                    var editToken = mapper.Map<EditTokenCommand>(tokenModel);
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
                return FailedOperation("Exp");
            }
        }
        #endregion

        #region StartTransaction
        [HttpPost]
        [Route("StartTransaction")]
        [SecretKeyCheck]
        public async Task<ActionResult<bool>> StartTransaction(StartTransactionDto data)
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
                    return ValidationError(new[,] { { "gateway", "Disable gateway" } });
                }
                #endregion

                var payment = paymentFactory.CreatePaymentFactory(gateway.Type, gateway.ModuleType);
                var startTransaction = await payment.StartTransaction(tokenModel, data);

                return startTransaction.IsSuccess;
            }
            catch (Exception exp)
            {
                return FailedOperation("Exp");
            }
        }
        #endregion
    }
}
