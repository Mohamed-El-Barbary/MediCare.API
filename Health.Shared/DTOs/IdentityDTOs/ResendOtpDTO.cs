using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Health.Shared.DTOs.IdentityDTOs
{
    public class ResendOtpDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = default!;
    }
}
