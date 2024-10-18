using MediatR;
using PaymentGateway.Domain;
using PaymentGateway.Domain.Payment.Gateways;

namespace PaymentGateway.Application.Payment.Gateways.Queries
{
    public class GetGatewaysByMerchantIdQuery : IRequest<List<Gateway>>
    {
        public required int MerchantId { get; set; }
    }

    public class GetGatewaysByMerchantIdQueryHandler(IUnitOfWork unitOfWork)
        : IRequestHandler<GetGatewaysByMerchantIdQuery, List<Gateway>>
    {
        public async Task<List<Gateway>> Handle(GetGatewaysByMerchantIdQuery request, CancellationToken cancellationToken)
        {
            return await unitOfWork.GatewayRepository.GetListByMerchantId(request.MerchantId);
        }
    }
}
