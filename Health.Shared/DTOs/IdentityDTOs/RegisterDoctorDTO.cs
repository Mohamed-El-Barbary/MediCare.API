using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Health.Shared.DTOs.IdentityDTOs
{
    public class RegisterDoctorDTO : RegisterUserDTO
    {
        public Gender Gender { get; init; } = default!;
        public string Specialization { get; init; } = default!;
        public int YearsOfExperience { get; init; } = default!;
        public string ClinicLocation { get; init; } = default!;
        public string? PhoneClinc { get; init; }
        public string MedicalLicenseNumber { get; set; } = default!;
        public AddressDTO Address { get; init; } = default!;
    }
}
