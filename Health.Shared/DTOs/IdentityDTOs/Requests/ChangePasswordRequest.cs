using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Shared.DTOs.IdentityDTOs.Requests
{
    public sealed record ChangePasswordRequest(
        string Password,
        string ConfirmPassword
    );
}
