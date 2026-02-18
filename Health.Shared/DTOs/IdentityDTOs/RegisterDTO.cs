using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Health.Shared.DTOs.IdentityDTOs
{
    public class RegisterDoctorDTO : RegisterUserDTO
    {
        public Gender Gender { get; init; }
        public string Specialization { get; init; } = default!;
        public int YearsOfExperience { get; init; }
        public string Bio { get; init; } = default!;
        public string ClinicLocation { get; init; } = default!;
        public string SyndicateCardUrl { get; init; } = default!;
        public string DoctorPictureUrl { get; init; } = default!;
        public AddressDTO Address { get; init; } = default!;
    }
}
