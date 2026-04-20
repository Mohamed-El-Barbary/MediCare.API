using AutoMapper;
using Health.Domain.Entities.AppointmentModule;
using Health.Shared.DTOs.AppointmentDTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Services.MappingProfiles.AppointmentMapping
{
    public class AppointmentMappingProfile : Profile
    {
        public AppointmentMappingProfile()
        {
            CreateMap<CreateAppointmentDTO, Appointment>()
                .ForMember(desc => desc.PatientProfileId, opt => opt.Ignore())
                .ForMember(desc => desc.Status, opt => opt.MapFrom(src => AppointmentStatus.Pending))
                .ForMember(desc => desc.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.AppointmentType, opt => opt.MapFrom(src => (AppointmentType)src.AppointmentType)); 
        }
    }
}
