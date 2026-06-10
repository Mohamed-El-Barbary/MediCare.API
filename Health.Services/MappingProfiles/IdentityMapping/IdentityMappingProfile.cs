using AutoMapper;
using Health.Domain.Entities.DoctorModule;
using Health.Domain.Entities.IdentityModule;
using Health.Domain.Entities.IdentityModule.Enums;
using Health.Domain.Entities.PatientModule;
using Health.Shared.DTOs.Enums;
using Health.Shared.DTOs.IdentityDTOs;
using Health.Shared.DTOs.IdentityDTOs.Requests;
using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Services.MappingProfiles.IdentityMapping
{
    internal class IdentityMappingProfile : Profile
    {

        public IdentityMappingProfile()
        {
            CreateMap<RegisterDoctorRequest, ApplicationUser>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email));

            CreateMap<RegisterDoctorRequest, DoctorProfile>()
                .ForMember(dest => dest.DoctorPictureUrl, opt => opt.Ignore())
                .ForMember(dest => dest.SyndicateCardUrl, opt => opt.Ignore())
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.JoinDate, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"));

            CreateMap<RegisterPatientRequest, ApplicationUser>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email));

            CreateMap<RegisterPatientRequest, PatientProfile>()
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.JoinDate, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
                .ForMember(dest => dest.PatientChronicDiseases,
                           opt => opt.MapFrom(src => src.ChronicDiseaseIds.Select(id => new PatientChronicDisease
                           {
                               ChronicDiseaseId = id
                           })))
                .AfterMap((src, dest) =>
                {
                    if (src.EmergencyContactName != null || src.EmergencyPhoneNumber != null)
                    {
                        dest.EmergencyContact ??= new EmergencyContact
                        {
                            ContactName = src.EmergencyContactName,
                            PhoneNumber = src.EmergencyPhoneNumber
                        };
                    }
                }); ;
            CreateMap<AddressDTO, Address>();

            CreateMap<OtpPurpose, OtpPurposeDTO>().ReverseMap();
        }

    }
}

