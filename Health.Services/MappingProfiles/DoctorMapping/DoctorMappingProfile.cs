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
            CreateMap<DoctorProfile, DoctorDTO>();
        }
    }
}
