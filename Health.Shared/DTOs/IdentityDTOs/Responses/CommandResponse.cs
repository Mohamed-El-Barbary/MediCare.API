using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Shared.DTOs.IdentityDTOs.Responses
{
    public record CommandResponse(
        bool Success,
        string Message
    );
}
