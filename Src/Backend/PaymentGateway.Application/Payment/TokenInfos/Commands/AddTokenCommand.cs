using AutoMapper;
using MediatR;
using PaymentGateway.Domain;
using PaymentGateway.Domain.Payment.TokenInfos;

namespace PaymentGateway.Application.Payment.TokenInfos.Commands
{
    public class AddTokenCommand : TokenInfo, IRequest<int>
    {
    }

    public class AddTokenCommandHandler (IUnitOfWork unitOfWork, IMapper mapper) 
        : IRequestHandler<AddTokenCommand, int>
    {
        public async Task<int> Handle(AddTokenCommand request, CancellationToken cancellationToken)
        {
            var entity = mapper.Map<TokenInfo>(request);
            return await unitOfWork.TokenInfoRepository.Insert(entity);
        }
    }
}
