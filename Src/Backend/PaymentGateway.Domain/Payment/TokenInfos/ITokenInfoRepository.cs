namespace PaymentGateway.Domain.Payment.TokenInfos
{
    public interface ITokenInfoRepository
    {
        Task<TokenInfo?> GetById(long id);
        Task<TokenInfo?> GetByToken(string token);
        Task<int> Insert(TokenInfo entity);
        Task<bool> Update(TokenInfo entity);
        Task<bool> Delete(long id);
        Task<bool> SoftDelete(long id);
    }
}