using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Health.Shared.DTOs.IdentityDTOs
{
    public record UserDTO(
        int Id,
        string Email,
        string DisplayName,
        string Role,
        string AccessToken,
        //[property: JsonIgnore]
        string RefreshToken,
        DateTime RefreshTokenExpiresOn
    );

}
