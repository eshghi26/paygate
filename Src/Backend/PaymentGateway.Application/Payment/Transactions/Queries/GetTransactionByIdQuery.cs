using MediatR;
using PaymentGateway.Domain;
using PaymentGateway.Domain.Payment.Transactions;

namespace PaymentGateway.Application.Payment.Transactions.Queries
{
    public class GetTransactionByIdQuery : IRequest<Transaction?>
    {
        public required long Id { get; set; }
    }

    public class GetTransactionByIdQueryHandler (IUnitOfWork unitOfWork)
        : IRequestHandler<GetTransactionByIdQuery, Transaction?>
    {
        public async Task<Transaction?> Handle(GetTransactionByIdQuery request, CancellationToken cancellationToken)
        {
            return await unitOfWork.TransactionRepository.GetById(request.Id);
        }
    }
}
