using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Shared.DTOs.IdentityDTOs
{
    public record UserDTO(
        string Id,
        string Email,
        string DisplayName,
        string Role,
        string AccessToken,
        DateTime RefreshTokenExpiresOn
    );

}
