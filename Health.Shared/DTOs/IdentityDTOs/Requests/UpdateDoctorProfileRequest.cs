using Health.Shared.DTOs.IdentityDTOs.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Shared.DTOs.IdentityDTOs.Requests
{
    public sealed class UpdateDoctorProfileRequest
    {
        public string? FirstName { get; init; }
        public string? LastName { get; init; }
        public string? PhoneNumber { get; init; }
        public Gender? Gender { get; init; }
        public string? Specialization { get; init; }
        public int? YearsOfExperience { get; init; }
        public string? Bio { get; init; }
        public string? ClinicLocation { get; init; }
        public string? PhoneClinic { get; init; }
        public AddressDTO? Address { get; init; }
        public IFormFile? DoctorPictureFile { get; init; }
        public IFormFile? SyndicateCardFile { get; init; }
    }
}
