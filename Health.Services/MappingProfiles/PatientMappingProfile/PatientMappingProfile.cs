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
            CreateMap<PatientProfile, PatientProfileResponce>();

            CreateMap<Appointment, UpcommingPatientAppointments>();

            CreateMap<Consultation, ConsultationDTO>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => ConsultationStatus.Scheduled.ToString()))
                 .ForMember(dest => dest.Type, opt => opt.MapFrom(src => ConsultationType.Online.ToString()));

            CreateMap<Address, AddressResponce>();

            CreateMap<BloodType, BloodTypeDTO>();
        }

    }
}
