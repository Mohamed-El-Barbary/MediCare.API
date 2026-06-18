using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Shared.DTOs.IdentityDTOs.Responses
{
    public record AccountProgressResponse(
        int CompletionPercentage,
        bool IsCompleted,
        List<string> MissingFields
    );
}
