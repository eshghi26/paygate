using MediatR;
using PaymentGateway.Domain;
using PaymentGateway.Domain.Payment.TokenInfos;

namespace PaymentGateway.Application.Payment.TokenInfos.Queries
{
    public class GetTokenInfoByTokenQuery : IRequest<TokenInfo?>
    {
        public required string Token { get; set; }
    }

    public class GetTokenInfoByTokenQueryHandler (IUnitOfWork unitOfWork) 
        : IRequestHandler<GetTokenInfoByTokenQuery, TokenInfo?>
    {
        public async Task<TokenInfo?> Handle(GetTokenInfoByTokenQuery request, CancellationToken cancellationToken)
        {
            return await unitOfWork.TokenInfoRepository.GetByToken(request.Token);
        }
    }
}