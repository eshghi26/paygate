using MediatR;
using PaymentGateway.Domain;
using PaymentGateway.Domain.Payment.Gateways;

namespace PaymentGateway.Application.Payment.Gateways.Queries
{
    public class GetGatewayByKeyQuery : IRequest<Gateway?>
    {
        public required string Key { get; set; }
    }

    public class GetGatewayByKeyQueryHandler (IUnitOfWork unitOfWork)
        : IRequestHandler<GetGatewayByKeyQuery, Gateway?>
    {
        public async Task<Gateway?> Handle(GetGatewayByKeyQuery request, CancellationToken cancellationToken)
        {
            return await unitOfWork.GatewayRepository.GetByKey(request.Key);
        }
    }
}
