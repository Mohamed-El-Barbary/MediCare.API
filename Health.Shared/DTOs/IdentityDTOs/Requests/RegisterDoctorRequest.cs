using Health.Shared.DTOs.IdentityDTOs.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Shared.DTOs.IdentityDTOs.Requests
{
    public sealed record RegisterDoctorRequest(
        string FirstName,
        string LastName,
        string PhoneNumber,
        string Email,
        string Password,
        string ConfirmPassword,
        Gender Gender,
        string Specialization,
        int YearsOfExperience,
        string ClinicLocation,
        string? PhoneClinc,
        string MedicalLicenseNumber,
        AddressDTO Address
    );
}
