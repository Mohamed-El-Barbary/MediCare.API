using Health.Domain.Entities.IdentityModule;
using Health.Domain.Entities.IdentityModule.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Domain.Contracts
{
    public interface IOtpRepository
    {
        Task<OtpEntry?> GetAsync(string email, OtpPurpose purpose);
        Task SetAsync(string email, OtpEntry entity, OtpPurpose purpose, TimeSpan ttl = default);
        Task DeleteAsync(string email, OtpPurpose purpose);
        Task<TimeSpan?> GetRemainingTtlAsync(string email, OtpPurpose purpose);

    }
}
