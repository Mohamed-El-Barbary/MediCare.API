using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Services.Abstraction
{
    public interface ICachService
    {
        Task<string?> GetAsync(string cachKey);

        Task SetAsync(string cacheKey, object cacheValue, TimeSpan timeToLive);
    }
}
