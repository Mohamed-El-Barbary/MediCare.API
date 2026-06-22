using AutoMapper;
using Health.Domain.Entities.AppointmentModule;
using Health.Domain.Entities.ConsultationModule;
using Health.Domain.Entities.ConsultationModule.Enums;
using Health.Domain.Entities.DoctorModule;
using Health.Domain.Entities.PatientModule;
using Health.Domain.Entities.PatientModule.enums;
using Health.Shared.DTOs.ConsultationDTOs;
using Health.Shared.DTOs.PatientDashboardDTOs;
using Health.Shared.DTOs.PatientDashboardDTOs.enumsForPatient;
using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Services.MappingProfiles.PatientMappingProfile
{
    public class PatientMappingProfile : Profile
    {
        public PatientMappingProfile()
        {
            CreateMap<PatientProfile, PatientProfileResponce>()
                .ForCtorParam("Gender", opt => opt.MapFrom(src => src.Gender.ToString()))
                .ForCtorParam("BloodType", opt => opt.MapFrom(src => src.BloodType != null ? src.BloodType.ToString(): null));

            CreateMap<Appointment, UpcommingPatientAppointments>()
                .ForCtorParam("Id", opt => opt.MapFrom(src => src.Id))
                .ForCtorParam("DoctorName", opt => opt.MapFrom(src => src.DoctorProfile.DisplayName))
                .ForCtorParam("DoctorSpecialization", opt => opt.MapFrom(src => src.DoctorProfile.Specialization))
                .ForCtorParam("AppointmentDate", opt => opt.MapFrom(src => src.CreatedAt))
                .ForCtorParam("StartTime", opt => opt.MapFrom(src => src.DoctorGeneratedSlots.StartTime))
                .ForCtorParam("EndTime", opt => opt.MapFrom(src => src.DoctorGeneratedSlots.EndTime))
                .ForCtorParam("Status", opt => opt.MapFrom(src => src.Status.ToString()))
                .ForCtorParam("Type", opt => opt.MapFrom(src => src.AppointmentType.ToString()));


            CreateMap<Consultation, ConsultationDTO>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => ConsultationStatus.Scheduled.ToString()))
                 .ForMember(dest => dest.Type, opt => opt.MapFrom(src => ConsultationType.Online.ToString()));

            CreateMap<Address, AddressResponce>();

            CreateMap<BloodType, BloodTypeDTO>();
        }

    }
}
