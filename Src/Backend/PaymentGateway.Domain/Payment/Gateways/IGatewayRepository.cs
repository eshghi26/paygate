namespace PaymentGateway.Domain.Payment.Gateways
{
    public interface IGatewayRepository
    {
        Task<Gateway?> GetById(long id);
        Task<List<Gateway>> GetListByMerchantId(int merchantId);
        Task<Gateway?> GetByKey(string key);
        Task<int> Insert(Gateway entity);
        Task<bool> Update(Gateway entity);
        Task<bool> Delete(long id);
        Task<bool> SoftDelete(long id);
    }
}