#region Using
using PaymentGateway.Domain;
using PaymentGateway.Domain.Payment.Gateways;
using PaymentGateway.Domain.Payment.Merchants;
using PaymentGateway.Domain.Payment.Resources;
using PaymentGateway.Domain.Payment.TokenInfos;
using PaymentGateway.Domain.Payment.UserCards;
using PaymentGateway.Domain.Security.Users;
using PaymentGateway.Infrastructure.Payment.Gateways;
using PaymentGateway.Infrastructure.Payment.Merchants;
using PaymentGateway.Infrastructure.Payment.Resources;
using PaymentGateway.Infrastructure.Payment.TokenInfos;
using PaymentGateway.Infrastructure.Payment.UserCards;
using PaymentGateway.Infrastructure.Security.Users;
#endregion

namespace PaymentGateway.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        #region User
        private UserRepository? _userRepository;
        public IUserRepository UserRepository
        {
            get
            {
                return _userRepository ??= new UserRepository();
            }
        }
        #endregion

        #region Merchant
        private MerchantRepository? _merchantRepository;
        public IMerchantRepository MerchantRepository
        {
            get
            {
                return _merchantRepository ??= new MerchantRepository();
            }
        }
        #endregion

        #region Gateway
        private GatewayRepository? _gatewayRepository;
        public IGatewayRepository GatewayRepository
        {
            get
            {
                return _gatewayRepository ??= new GatewayRepository();
            }
        }
        #endregion

        #region TokenInfo
        private TokenInfoRepository? _tokenInfoRepository;
        public ITokenInfoRepository TokenInfoRepository
        {
            get
            {
                return _tokenInfoRepository ??= new TokenInfoRepository();
            }
        }
        #endregion

        #region Resource
        private ResourceRepository? _resourceRepository;
        public IResourceRepository ResourceRepository
        {
            get
            {
                return _resourceRepository ??= new ResourceRepository();
            }
        }
        #endregion

        #region UserCard
        private UserCardRepository? _userCardRepository;
        public IUserCardRepository UserCardRepository
        {
            get
            {
                return _userCardRepository ??= new UserCardRepository();
            }
        }
        #endregion
    }
}