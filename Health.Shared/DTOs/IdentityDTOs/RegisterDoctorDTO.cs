using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Health.Shared.DTOs.IdentityDTOs
{
    public class RegisterDoctorDTO : RegisterUserDTO
    {
        public string Gender { get; init; } = default!;
        public string Specialization { get; init; } = default!;
        public int YearsOfExperience { get; init; }
        public string Bio { get; init; } = default!;
        public string ClinicLocation { get; init; } = default!;
        public string PhoneClinc { get; init; } = default!;
        public IFormFile DoctorPictureFile { get; init; } = default!;
        public IFormFile SyndicateCardFile { get; init; } = default!;
        public AddressDTO Address { get; init; } = default!;
    }
}
