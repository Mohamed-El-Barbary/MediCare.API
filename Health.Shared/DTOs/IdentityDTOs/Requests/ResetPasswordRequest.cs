using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Shared.DTOs.IdentityDTOs.Requests
{
    public sealed record ResetPasswordRequest(
        string Email,
        string Password,
        string ConfirmPassword
    );
}
