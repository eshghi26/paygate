using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using PaymentGateway.Domain;
using PaymentGateway.Domain.Payment.UserCards;

namespace PaymentGateway.Application.Payment.UserCards.Commands
{
    public class AddUserCardCommand : UserCard, IRequest<long?>
    {
    }

    public class AddUserCardCommandHandler (IUnitOfWork unitOfWork, IMapper mapper, 
        ILogger<AddUserCardCommandHandler> logger) : IRequestHandler<AddUserCardCommand, long?>
    {
        public async Task<long?> Handle(AddUserCardCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var cards = await unitOfWork.UserCardRepository.GetByUserId(request.UserId, request.MerchantId);

                var card = cards?.FirstOrDefault(c => c.Pan == request.Pan);

                if (card != null)
                {
                    card.Cvv2 = request.Cvv2;
                    card.ExpYear = request.ExpYear;
                    card.ExpMonth = request.ExpMonth;

                    var updated = await unitOfWork.UserCardRepository.Update(card);

                    return updated ? card.Id : null;
                }

                var entity = mapper.Map<UserCard>(request);
                return await unitOfWork.UserCardRepository.Insert(entity);
            }
            catch (Exception exp)
            {
                logger.LogError(exp, exp.Message);
                return null;
            }
        }
    }
}
