namespace PaymentGateway.Domain.Payment.Merchants
{
    public interface IMerchantRepository
    {
        Task<Merchant?> GetById(long id);
        Task<int> Insert(Merchant entity);
        Task<bool> Update(Merchant entity);
        Task<bool> Delete(long id);
        Task<bool> SoftDelete(long id);
    }
}
