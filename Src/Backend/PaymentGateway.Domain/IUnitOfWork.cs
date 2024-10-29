using PaymentGateway.Domain.Payment.Gateways;
using PaymentGateway.Domain.Payment.Merchants;
using PaymentGateway.Domain.Payment.Resources;
using PaymentGateway.Domain.Payment.TokenInfos;
using PaymentGateway.Domain.Payment.Transactions;
using PaymentGateway.Domain.Payment.UserCards;
using PaymentGateway.Domain.Security.Users;

namespace PaymentGateway.Domain
{
    public interface IUnitOfWork
    {
        IUserRepository UserRepository { get; }
        IMerchantRepository MerchantRepository { get; }
        IGatewayRepository GatewayRepository { get; }
        ITokenInfoRepository TokenInfoRepository { get; }
        IResourceRepository ResourceRepository { get; }
        IUserCardRepository UserCardRepository { get; }
        ITransactionRepository TransactionRepository { get; }
    }
}
