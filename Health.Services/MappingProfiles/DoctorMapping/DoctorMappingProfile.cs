using AutoMapper;
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
                     .ForMember(Dest => Dest.DoctorScheduleDTO, opt => opt.MapFrom(s => s.DoctorProfile.DoctorSchedule))
                     .ForMember(Dest => Dest.GeneratedSlotsDTO, opt => opt.MapFrom(s => s.DoctorProfile.DoctorGeneratedSlots))
                     .ForMember(d => d.DoctorPictureUrl, O => O.MapFrom<DoctorPictureUrlResolver>());

        }
    }
}
