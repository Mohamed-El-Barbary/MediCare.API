using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Health.Shared.DTOs.IdentityDTOs.Requests
{
    public sealed record ResendOtpRequest(
        [Required]
        [EmailAddress]
        string Email 
    );
}
