namespace PaymentGateway.Domain.Payment.UserCards
{
    public interface IUserCardRepository
    {
        Task<UserCard?> GetById(long id);
        Task<List<UserCard>?> GetByUserId(string userId, int merchantId);
        Task<int> Insert(UserCard entity);
        Task<bool> Update(UserCard entity);
        Task<bool> Delete(long id);
        Task<bool> SoftDelete(long id);
    }
}
