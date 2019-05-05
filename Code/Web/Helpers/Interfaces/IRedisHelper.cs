using System;

namespace MultiTenant.Helpers.Interfaces
{
    public interface IRedisHelper
    {
        string Get(string key);
        void Set( string key, string value, double? expirationInSeconds = null, TimeSpan? slidingExpiration = null, DateTimeOffset? absoluteExpiration = null);
    }
}