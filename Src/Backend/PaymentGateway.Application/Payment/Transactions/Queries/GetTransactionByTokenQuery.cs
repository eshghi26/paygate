using MediatR;
using PaymentGateway.Domain;
using PaymentGateway.Domain.Payment.Transactions;

namespace PaymentGateway.Application.Payment.Transactions.Queries
{
    public class GetTransactionByTokenQuery : IRequest<Transaction?>
    {
        public required Guid Token { get; set; }
    }

    public class GetTransactionByTokenQueryHandler (IUnitOfWork unitOfWork)
        : IRequestHandler<GetTransactionByTokenQuery, Transaction?>
    {
        public async Task<Transaction?> Handle(GetTransactionByTokenQuery request, CancellationToken cancellationToken)
        {
            return await unitOfWork.TransactionRepository.GetByToken(request.Token);
        }
    }
}
