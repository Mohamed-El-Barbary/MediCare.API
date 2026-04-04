using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Shared.DTOs.IdentityDTOs
{
    public class ChangePasswordDTO
    {
        public string Password { get; set; } = default!;
        public string ConfirmPassword { get; set; } = default!;
    }
}
