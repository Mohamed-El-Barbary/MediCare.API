using Health.Shared.DTOs.IdentityDTOs.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Shared.DTOs.IdentityDTOs.Requests
{
     public sealed record RegisterPatientRequest(
         string FirstName ,
         string LastName,
         string PhoneNumber, 
         string Email ,
         string Password, 
         string ConfirmPassword,
         Gender Gender,
         DateTime DateOfBirth,
         AddressDTO Address,
         ICollection<int> ChronicDiseaseIds,
         string? EmergencyContactName,
         string? EmergencyPhoneNumber,
         int? BloodType
    );
}
