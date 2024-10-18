using MediatR;
using PaymentGateway.Domain;
using PaymentGateway.Domain.Payment.UserCards;

namespace PaymentGateway.Application.Payment.UserCards.Queries
{
    public class GetUserCardsByUserIdQuery : IRequest<List<UserCard>?>
    {
        public required string UserId { get; set; }
        public int MerchantId { get; set; }
    }

    public class GetUserCardsByUserIdQueryHandler (IUnitOfWork unitOfWork)
        : IRequestHandler<GetUserCardsByUserIdQuery, List<UserCard>?>
    {
        public async Task<List<UserCard>?> Handle(GetUserCardsByUserIdQuery request, CancellationToken cancellationToken)
        {
            return await unitOfWork.UserCardRepository.GetByUserId(request.UserId, request.MerchantId);
        }
    }
}
