using MediatR;
using PaymentGateway.Domain;
using PaymentGateway.Domain.Payment.Resources;

namespace PaymentGateway.Application.Payment.Resources.Queries
{
    public class GetResourceByNameQuery : IRequest<Resource?>
    {
        public required string Name { get; set; }
    }

    public class GetResourceByNameQueryHandler (IUnitOfWork unitOfWork)
        : IRequestHandler<GetResourceByNameQuery, Resource?>
    {
        public async Task<Resource?> Handle(GetResourceByNameQuery request, CancellationToken cancellationToken)
        {
            return await unitOfWork.ResourceRepository.GetByName(request.Name);
        }
    }
}
