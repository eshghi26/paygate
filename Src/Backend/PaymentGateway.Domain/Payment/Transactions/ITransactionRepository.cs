namespace PaymentGateway.Domain.Payment.Transactions
{
    public interface ITransactionRepository
    {
        Task<Transaction?> GetById(long id);
        Task<Transaction?> GetByToken(Guid token);
        Task<Transaction?> GetByTransactionCode(string transactionCode);
        Task<long> Insert(Transaction entity);
        Task<bool> Update(Transaction entity);
        Task<bool> Delete(long id);
    }
}