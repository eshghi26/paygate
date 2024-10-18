using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace PaymentGateway.Infrastructure.Cache
{
    public class Redis(IDistributedCache redis) : IRedis
    {
        #region Get
        public async Task<T?> GetAsync<T>(string key)
        {
            var value = await redis.GetStringAsync(key);
            if (!string.IsNullOrEmpty(value))
            {
                return JsonConvert.DeserializeObject<T>(value);
            }

            return default;
        }

        public T? Get<T>(string key)
        {
            var value = redis.GetString(key);
            if (!string.IsNullOrEmpty(value))
            {
                return JsonConvert.DeserializeObject<T>(value);
            }

            return default;
        }

        public async Task<string?> GetStringAsync(string key)
        {
            var value = await redis.GetStringAsync(key);
            if (!string.IsNullOrEmpty(value))
            {
                return value;
            }

            return default;
        }

        public string? GetString(string key)
        {
            var value = redis.GetString(key);
            if (!string.IsNullOrEmpty(value))
            {
                return value;
            }

            return default;
        }
        #endregion

        #region Set
        public async Task SetAsync<T>(string key, T value, DateTimeOffset? expiration = null)
        {
            await redis.SetStringAsync(key, JsonConvert.SerializeObject(value),
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpiration = expiration
                });
        }

        public void Set<T>(string key, T value, DateTimeOffset? expiration = null)
        {
            redis.SetString(key, JsonConvert.SerializeObject(value),
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpiration = expiration
                });
        }

        public async Task SetStringAsync(string key, string value, DateTimeOffset? expirationTime = null)
        {
            await redis.SetStringAsync(key, value, new DistributedCacheEntryOptions
            {
                AbsoluteExpiration = expirationTime
            });
        }

        public void SetString(string key, string value, DateTimeOffset? expirationTime = null)
        {
            redis.SetString(key, value, new DistributedCacheEntryOptions
            {
                AbsoluteExpiration = expirationTime
            });
        }
        #endregion

        #region Delete
        public async Task DeleteAsync(string key)
        {
            await redis.RemoveAsync(key);
        }

        public void Delete(string key)
        {
            redis.Remove(key);
        }
        #endregion
    }
}