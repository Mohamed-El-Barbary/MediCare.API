using Health.Shared.DTOs.IdentityDTOs.Enums;
using Health.Shared.DTOs.PatientDashboardDTOs.enumsForPatient;
using System.Net;

namespace Health.Shared.DTOs.PatientDashboardDTOs
{
    public sealed record PatientProfileResponce
    (
         string DisplayName,
         string Gender ,
         DateTime DateOfBirth ,
         DateTime JoinDate,
         AddressResponce Address,
         string? BloodType
    );
}