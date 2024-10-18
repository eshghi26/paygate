using AutoMapper;
using MediatR;
using PaymentGateway.Domain;
using PaymentGateway.Domain.Payment.TokenInfos;

namespace PaymentGateway.Application.Payment.TokenInfos.Commands
{
    public class EditTokenCommand : TokenInfo, IRequest<bool>
    {
    }

    public class EditTokenCommandHandler (IUnitOfWork unitOfWork, IMapper mapper) 
        : IRequestHandler<EditTokenCommand, bool>
    {
        public async Task<bool> Handle(EditTokenCommand request, CancellationToken cancellationToken)
        {
            var entity = mapper.Map<TokenInfo>(request);
            return await unitOfWork.TokenInfoRepository.Update(entity);
        }
    }
}
