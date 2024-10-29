using AutoMapper;
using MediatR;
using PaymentGateway.Domain;
using PaymentGateway.Domain.Payment.Transactions;

namespace PaymentGateway.Application.Payment.Transactions.Commands
{
    public class AddTransactionCommand : Transaction, IRequest<long?>
    {
    }

    public class AddTransactionCommandHandler (IUnitOfWork unitOfWork, IMapper mapper)
        : IRequestHandler<AddTransactionCommand, long?>
    {
        public async Task<long?> Handle(AddTransactionCommand request, CancellationToken cancellationToken)
        {
            var entity = mapper.Map<Transaction>(request);
            return await unitOfWork.TransactionRepository.Insert(entity);
        }
    }
}