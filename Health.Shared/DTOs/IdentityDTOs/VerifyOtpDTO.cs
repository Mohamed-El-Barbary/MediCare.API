using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Shared.DTOs.IdentityDTOs
{
    public class VerifyOtpDTO
    {
        public string Email { get; init; } = default!;
        public string OtpCode { get; init; } = default!;
    }
}
