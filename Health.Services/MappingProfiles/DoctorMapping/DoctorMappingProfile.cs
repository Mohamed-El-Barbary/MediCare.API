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
            CreateMap<DoctorProfile, DoctorDTO>();


            CreateMap<DoctorScheduleDTO, DoctorSchedule>()
           .ForMember(dest => dest.DoctorProfileId,
                      opt => opt.Ignore());

            CreateMap<DoctorSchedule, DoctorSceduleToReturn>();

        }
    }
}
