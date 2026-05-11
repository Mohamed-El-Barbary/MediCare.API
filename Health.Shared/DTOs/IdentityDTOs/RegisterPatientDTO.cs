using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Health.Shared.DTOs.IdentityDTOs
{
    public class RegisterPatientDTO : RegisterUserDTO
    {
        public Gender Gender { get; init; }
        public DateTime DateOfBirth { get; init; }

        // Address
        public AddressDTO Address { get; init; } = null!;

        // Chronic Diseases selected (IDs from UI)
        public ICollection<int> ChronicDiseaseIds { get; init; } = [];
        public string? EmergencyContactName { get; init; }
        public string? EmergencyPhoneNumber { get; init; }
        public int? BloodType { get; init; }
    }
}
