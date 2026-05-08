using Health.Domain.Contracts;
using Health.Domain.Entities.IdentityModule;
using Health.Domain.Entities.IdentityModule.Enums;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Health.Persistence.Repositories
{
    public class OtpRepository : IOtpRepository
    {
        private readonly IDatabase _database;
        public OtpRepository(IConnectionMultiplexer connection)
        {
            _database = connection.GetDatabase();
        }

        public async Task<OtpEntry?> GetAsync(string email, OtpPurpose purpose)
        {
            var key = generateOtpKey(email, purpose);
            var otpData = await _database.StringGetAsync(key);

            return otpData.IsNullOrEmpty
                ? null
                : JsonSerializer.Deserialize<OtpEntry>((string)otpData!);

        }

        public async Task SetAsync(string email, OtpEntry entity, OtpPurpose purpose, TimeSpan ttl = default)
        {
            var key = generateOtpKey(email, purpose);
            var jsonOtp = JsonSerializer.Serialize(entity);
            await _database.StringSetAsync(key, jsonOtp, (ttl == default) ? TimeSpan.FromMinutes(8) : ttl);
        }

        public async Task DeleteAsync(string email, OtpPurpose purpose)
        {
            var key = generateOtpKey(email, purpose);
            await _database.KeyDeleteAsync(key);
        }

        public async Task<TimeSpan?> GetRemainingTtlAsync(string email, OtpPurpose purpose)
        {
            var key = generateOtpKey(email, purpose);
            return await _database.KeyTimeToLiveAsync(key);
        }

        #region HelperMethod
        private string generateOtpKey(string email, OtpPurpose otpPurpose)
            => $"otp:{otpPurpose}:{email.ToLowerInvariant()}";
        #endregion
    }
}
