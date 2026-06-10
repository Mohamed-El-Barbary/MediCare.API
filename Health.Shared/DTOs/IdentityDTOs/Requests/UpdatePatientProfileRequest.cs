using Health.Shared.DTOs.IdentityDTOs.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Shared.DTOs.IdentityDTOs.Requests
{
    public sealed class UpdatePatientProfileRequest {
        public string? FirstName { get; init; }
        public string? LastName { get; init; }
        public string? PhoneNumber { get; init; }
        public string? DisplayName { get; init; }
        public Gender? Gender { get; init; }
        public DateTime? DateOfBirth { get; init; }
        public AddressDTO? Address { get; init; }
        public ICollection<int>? ChronicDiseaseIds { get; init; }
    }
}
