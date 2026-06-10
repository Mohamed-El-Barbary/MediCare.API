using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Health.Shared.DTOs.IdentityDTOs.Requests
{
    public sealed record ForgotPasswordRequest(
        [Required]
        [EmailAddress]
        string Email
    );
}
