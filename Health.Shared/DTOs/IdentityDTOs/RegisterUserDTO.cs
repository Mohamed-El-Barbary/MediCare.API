using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Shared.DTOs.IdentityDTOs
{
    public abstract class RegisterUserDTO
    {
        public string FirstName { get; init; } = default!;
        public string LastName { get; init; } = default!;
        public string PhoneNumber { get; init; } = default!;
        public string Email { get; init; } = default!;
        public string Password { get; init; } = default!;
        public string ConfirmPassword { get; init; } = default!;
    }

}
