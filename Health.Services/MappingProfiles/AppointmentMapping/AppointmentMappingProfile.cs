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

            CreateMap<Appointment, PatientAppointmentDTO>()
                .ForMember(desc => desc.DoctorName , op => op.MapFrom(src => src.DoctorProfile.DisplayName))
                .ForMember(desc => desc.DoctorSpecialization , op => op.MapFrom(src => src.DoctorProfile.Specialization))
                .ForMember(desc => desc.AppointmentDate , op => op.MapFrom(src => src.DoctorGeneratedSlots.SlotDate.ToString("yyyy-MM-dd")))
                .ForMember(desc => desc.StartTime , op => op.MapFrom(src => src.DoctorGeneratedSlots.StartTime))
                .ForMember(desc => desc.EndTime , op => op.MapFrom(src => src.DoctorGeneratedSlots.EndTime))
                .ForMember(desc => desc.Status, op => op.MapFrom(src => src.Status.ToString()))
                .ForMember(desc => desc.Type, op => op.MapFrom(src => src.AppointmentType.ToString()));

            CreateMap<Appointment , DoctorAppointmentDTO>()
                .ForMember(desc => desc.PatientName , op => op.MapFrom(src => src.PatientProfile.DisplayName))
                .ForMember(desc => desc.AppointmentDate , op => op.MapFrom(src => src.DoctorGeneratedSlots.SlotDate.ToString("yyy-MM-dd")))
                .ForMember(desc => desc.StartTime, op => op.MapFrom(src => src.DoctorGeneratedSlots.StartTime))
                .ForMember(desc => desc.EndTime, op => op.MapFrom(src => src.DoctorGeneratedSlots.EndTime))
                .ForMember(desc => desc.Status, op => op.MapFrom(src => src.Status.ToString()))
                .ForMember(desc => desc.Type, op => op.MapFrom(src => src.AppointmentType.ToString()));
        }
    }
}
