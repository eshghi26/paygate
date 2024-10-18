using PaymentGateway.Domain.Base;

namespace PaymentGateway.Domain.Security.Users
{
    public interface IUserRepository
    {
        Task<(List<User> Data, int TotalCount)> GetList(FetchOption filter);
        Task<User?> GetById(long id);
        Task<User?> Login(string userName, string password);
        Task<User?> GetByRefreshTokenSerial(string refreshTokenSerial);
        Task<bool> UserNameIsExist(string userName, int id);
        Task<bool> EmailIsExist(string email, int id);
        Task<int> Insert(User entity);
        Task<bool> Update(User entity);
        Task<bool> UpdateRefreshToken(int id, string? refreshTokenSerial, DateTime? refreshTokenExpiryDate);
        Task<bool> UpdatePassword(int id, string password);
        Task<bool> Delete(long id);
        Task<bool> SoftDelete(long id);
    }
}
