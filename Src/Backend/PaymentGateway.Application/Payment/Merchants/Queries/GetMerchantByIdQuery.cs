using MediatR;
using PaymentGateway.Domain;
using PaymentGateway.Domain.Payment.Merchants;

namespace PaymentGateway.Application.Payment.Merchants.Queries
{
    public class GetMerchantByIdQuery : IRequest<Merchant?>
    {
        public required int Id { get; set; }
    }

    public class GetMerchantByIdQueryHandler(IUnitOfWork unitOfWork): 
        IRequestHandler<GetMerchantByIdQuery, Merchant?>
    {
        public async Task<Merchant?> Handle(GetMerchantByIdQuery request, CancellationToken cancellationToken)
        {
            return await unitOfWork.MerchantRepository.GetById(request.Id);
        }
    }
}
