using MediatR;
using PaymentGateway.Domain;
using PaymentGateway.Domain.Payment.Gateways;

namespace PaymentGateway.Application.Payment.Gateways.Queries
{
    public class GetGatewayByIdQuery : IRequest<Gateway?>
    {
        public required int Id { get; set; }
    }

    public class GetGatewayByIdQueryHandler (IUnitOfWork unitOfWork)
        : IRequestHandler<GetGatewayByIdQuery, Gateway?>
    {
        public async Task<Gateway?> Handle(GetGatewayByIdQuery request, CancellationToken cancellationToken)
        {
            return await unitOfWork.GatewayRepository.GetById(request.Id);
        }
    }
}
