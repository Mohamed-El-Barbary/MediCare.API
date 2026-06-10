using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Shared.DTOs.IdentityDTOs.Responses
{
    public sealed record DoctorProfileResponse(
       int Id,
       string FullName,
       string Bio,
       string Specialty,
       int YearsOfExperience,
       int AccountPercentage
   );
}
