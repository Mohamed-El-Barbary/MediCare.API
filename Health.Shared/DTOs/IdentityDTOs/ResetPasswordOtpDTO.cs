using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Shared.DTOs.IdentityDTOs
{
    public class ResetPasswordOtpDTO
    {
        public string Email { get; init; } = default!;
        public string Password { get; init; } = default!;
    }
}
