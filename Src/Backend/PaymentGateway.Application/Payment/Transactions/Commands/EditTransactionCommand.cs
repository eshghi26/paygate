using AutoMapper;
using MediatR;
using PaymentGateway.Domain;
using PaymentGateway.Domain.Payment.Transactions;

namespace PaymentGateway.Application.Payment.Transactions.Commands
{
    public class EditTransactionCommand : Transaction, IRequest<bool>
    {
    }

    public class EditTransactionCommandHandler (IUnitOfWork unitOfWork, IMapper mapper)
        : IRequestHandler<EditTransactionCommand, bool>
    {
        public async Task<bool> Handle(EditTransactionCommand request, CancellationToken cancellationToken)
        {
            var entity = mapper.Map<Transaction>(request);
            return await unitOfWork.TransactionRepository.Update(entity);
        }
    }
}