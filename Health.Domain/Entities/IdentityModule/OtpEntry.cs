using Health.Domain.Entities.IdentityModule.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Domain.Entities.IdentityModule
{
    public class OtpEntry
    {
        /// <summary>
        /// SHA-256 hash of the OTP code. Never store the plain value.
        /// </summary>
        public string HashedCode { get; set; } = default!;
        public bool IsVerified { get; set; }
        public DateTime CreatedAt { get; set; }
        /// <summary>
        /// Absolute expiry timestamp — used for application-level expiry checks
        /// independent of the Redis TTL.
        /// </summary>
        public DateTime ExpiresAt { get; set; }
        public string? IpAddress { get; set; }
        public int AttemptCount { get; set; }
        public OtpPurpose Purpose { get; set; }
    }
}
