using PaymentGateway.Domain.Payment.Gateways;
using PaymentGateway.Domain.Payment.TokenInfos;
using PaymentGateway.Domain.Payment.TokenInfos.Dto;
using PaymentGateway.Domain.Payment.Transactions;

namespace PaymentGateway.PaymentApi.PaymentType.Factory
{
    public interface IPayment
    {
        Task<(bool IsSuccess, TokenInfo? Data, string? ErrorMessage)> 
            GetTransactionCode(Gateway gateway, TokenInfo tokenModel);


        Task<(bool IsSuccess, string? ErrorMessage)>
            StartTransaction(TokenInfo tokenModel, StartTransactionDto data);


        Task<(bool IsSuccess, string? ErrorMessage)>
            FinishTransaction(TokenInfo tokenModel, string pin);


        Task<(bool IsSuccess, string? ErrorMessage)>
            CancelTransaction(TokenInfo tokenModel);

        Task<(bool IsSuccess, string? ErrorMessage)>
            VerifyTransaction(Gateway gateway, Transaction transaction);
    }
}