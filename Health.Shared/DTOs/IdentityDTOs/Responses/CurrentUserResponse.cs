using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Shared.DTOs.IdentityDTOs.Responses
{
    public record CurrentUserResponse(
        int Id,
        string FullName,
        string Email,
        string Role
    );
}
