using AutoMapper;
using Health.Domain.Entities.DoctorModule;
using Health.Services.Aggregates;
using Health.Shared.DTOs.DoctorDTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Services.MappingProfiles.DoctorMapping
{
    internal class DoctorMappingProfile : Profile
    {

        public DoctorMappingProfile()
        {
            CreateMap<DoctorAggregate, DoctorDTO>()
                     .ForMember(d => d.DoctorId, O => O.MapFrom(s => s.DoctorProfile.Id))
                     .ForMember(d => d.FisrtName, O => O.MapFrom(s => s.ApplicationUser.FirstName))
                     .ForMember(d => d.LastName, O => O.MapFrom(s => s.ApplicationUser.LastName))
                     .ForMember(d => d.Specialization, O => O.MapFrom(s => s.DoctorProfile.Specialization))
                     .ForMember(d => d.YearsOfExperience, o => o.MapFrom(s => s.DoctorProfile.YearsOfExperience))
                     .ForMember(d => d.Bio, O => O.MapFrom(s => s.DoctorProfile.Bio))
                     .ForMember(d => d.Rating , O=>O.MapFrom(s => s.DoctorProfile.Rating))
                     .ForMember(d => d.PhoneNumber , O=>O.MapFrom(s => s.ApplicationUser.PhoneNumber))
                     .ForMember(d => d.DoctorPictureUrl, O => O.MapFrom<DoctorPictureUrlResolver>());

            CreateMap<DoctorSchedule, DoctorScheduleDTO>();
            CreateMap<DoctorGeneratedSlots, GeneratedSlotsDTO>();
        }
    }
}
