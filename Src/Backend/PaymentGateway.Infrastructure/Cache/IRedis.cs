namespace PaymentGateway.Infrastructure.Cache
{
    public interface IRedis
    {
        Task<T?> GetAsync<T>(string key);
        T? Get<T>(string key);
        Task<string?> GetStringAsync(string key);
        string? GetString(string key);
        Task SetAsync<T>(string key, T value, DateTimeOffset? expiration = null);
        void Set<T>(string key, T value, DateTimeOffset? expiration = null);
        Task SetStringAsync(string key, string value, DateTimeOffset? expirationTime = null);
        void SetString(string key, string value, DateTimeOffset? expirationTime = null);
        Task DeleteAsync(string key);
        void Delete(string key);
    }
}
