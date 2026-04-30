using AutoMapper;
using Health.Domain.Entities.DoctorModule;
using Health.Services.Aggregates;
using Health.Shared.DTOs.DoctorDTOs;
using Health.Shared.DTOs.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Services.MappingProfiles.DoctorMapping
{
    internal class DoctorMappingProfile : Profile
    {

        public DoctorMappingProfile()
        {
            CreateMap<DoctorProfile, DoctorDTO>()
                    .ForMember(dest => dest.DoctorScheduleDTO, opt => opt.MapFrom(src => src.DoctorSchedule))
                 .ForMember(dest => dest.GeneratedSlotsDTO,opt => opt.MapFrom(src => src.DoctorGeneratedSlots));


            CreateMap<DoctorScheduleDTO, DoctorSchedule>()
           .ForMember(dest => dest.DoctorProfileId,
                      opt => opt.Ignore());

            CreateMap<DoctorSchedule, DoctorScheduleDTO>();
            CreateMap<DoctorGeneratedSlots, GeneratedSlotsDTO>();


            CreateMap<DoctorSchedule, DoctorSceduleToReturn>();

        }
    }
}
