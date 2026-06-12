using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Shared.DTOs.DoctorDTOs
{
    public sealed record DoctorProfileSummaryResponse(
        string FullName,
        string Specialization,
        int CompletionPercentage,
        string Status,
        decimal Rating
    );
}
