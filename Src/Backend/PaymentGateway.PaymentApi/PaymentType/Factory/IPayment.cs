using PaymentGateway.Domain.Payment.Gateways;
using PaymentGateway.Domain.Payment.TokenInfos;
using PaymentGateway.Domain.Payment.TokenInfos.Dto;

namespace PaymentGateway.PaymentApi.PaymentType.Factory
{
    public interface IPayment
    {
        Task<(bool IsSuccess, 
                GetTransactionCodeModel? Data, 
                string? ErrorMessage)> 
            GetTransactionCode(Gateway gateway, decimal amount);

        Task<(bool IsSuccess, string? ErrorMessage)>
            StartTransaction(TokenInfo tokenModel, StartTransactionDto data);
    }
}