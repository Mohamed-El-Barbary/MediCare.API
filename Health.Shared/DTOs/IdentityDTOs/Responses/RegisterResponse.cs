using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Shared.DTOs.IdentityDTOs.Responses
{
    public record RegisterResponse(
        int ProfileId,
        string Role,
        string Message,
        TokenResponse Token
    );
}
