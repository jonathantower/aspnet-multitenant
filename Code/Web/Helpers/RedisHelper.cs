using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using MultiTenant.Helpers.Interfaces;
using System;

namespace MultiTenant.Helpers
{
    public class RedisHelper : IRedisHelper
    {
        private readonly IDistributedCache _cache;
        private readonly HttpContext _httpContext;

        public RedisHelper(
            IDistributedCache cache, 
            IHttpContextAccessor httpContextAccessor)
        {
            _cache = cache;
            _httpContext = httpContextAccessor?.HttpContext;
        }

        public string Get(string key)
        {
            string tenantKey = BuildTenantKey(key);
            return _cache.GetString(tenantKey);
        }

        public void Set(
            string key, 
            string value, 
            double? expirationInSeconds = null, 
            TimeSpan? slidingExpiration = null,
            DateTimeOffset? absoluteExpiration = null)
        {
            var tenantKey = BuildTenantKey(key);
            var opts = BuildOptions(expirationInSeconds, slidingExpiration, absoluteExpiration);
            _cache.SetString(tenantKey, value, opts);
        }

        public void Remove(string key)
        {
            var tenantKey = BuildTenantKey(key);
            _cache.Remove(tenantKey);
        }

        private string BuildTenantKey(string key)
        {
            // build expiration options
            var user = _httpContext.User;
            var tenantId = ClaimsHelper.GetClaim<int>(user, "tenantid");
            var tenantKey = $"t{tenantId}_{key}";
            return tenantKey;
        }

        private static DistributedCacheEntryOptions BuildOptions(double? expirationInSeconds, TimeSpan? slidingExpiration, DateTimeOffset? absoluteExpiration)
        {
            return new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expirationInSeconds == null
                                ? (TimeSpan?)null
                                : TimeSpan.FromSeconds(expirationInSeconds.Value),

                AbsoluteExpiration = absoluteExpiration == null
                                ? (DateTimeOffset?)null
                                : absoluteExpiration.Value,

                SlidingExpiration = slidingExpiration == null
                                ? (TimeSpan?)null
                                : slidingExpiration.Value
            };
        }
    }
}
