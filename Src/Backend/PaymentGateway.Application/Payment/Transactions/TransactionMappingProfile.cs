using AutoMapper;
using PaymentGateway.Application.Payment.Transactions.Commands;
using PaymentGateway.Domain.Payment.Transactions;

namespace PaymentGateway.Application.Payment.Transactions
{
    public class TransactionMappingProfile : Profile
    {
        public TransactionMappingProfile()
        {
            CreateMap<EditTransactionCommand, Transaction>().ReverseMap();
        }
    }
}
