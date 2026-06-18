using Health.Shared.DTOs.IdentityDTOs.Enums;
using Health.Shared.DTOs.PatientDashboardDTOs.enumsForPatient;
using System.Net;

namespace Health.Shared.DTOs.PatientDashboardDTOs
{
    public sealed record PatientProfileResponce
    (
         string DisplayName,
         Gender Gender ,
         DateTime DateOfBirth ,
         DateTime JoinDate,
         AddressResponce Address,
         BloodTypeDTO? BloodType
    );
}