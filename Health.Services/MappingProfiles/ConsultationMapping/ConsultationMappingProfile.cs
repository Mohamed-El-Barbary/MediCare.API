using AutoMapper;
using Health.Domain.Entities.ConsultationModule;
using Health.Domain.Entities.ConsultationModule.Enums;
using Health.Shared.DTOs.ConsultationDTOs;
using Health.Shared.DTOs.ConsultationSessionDTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Services.MappingProfiles.ConsultationMapping
{
    internal class ConsultationMappingProfile : Profile
    {

        public ConsultationMappingProfile()
        {
            CreateMap<CreateConsultationDTO, Consultation>()
                .ForMember(dest => dest.ScheduledAt, opt => opt.MapFrom(src => DateTime.UtcNow));
            CreateMap<Consultation, ConsultationDTO>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => ConsultationStatus.Scheduled.ToString()))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => ConsultationType.Online.ToString()));
            CreateMap<Consultation, ConsultationSummaryDTO>()
                .ForMember(dest => dest.DoctorName, opt => opt.MapFrom(src => src.Doctor.DisplayName))
                .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.Patient.DisplayName));

            CreateMap<CreatePrescriptionDTO, Prescription>();
            CreateMap<UpdatePrescriptionDTO, Prescription>();
            CreateMap<Prescription, PrescriptionDTO>();
            CreateMap<PrescriptionItem, PrescriptionItemDTO>();
            CreateMap<CreatePrescriptionItemDTO, PrescriptionItem>();
            CreateMap<UpdatePrescriptionItemDTO, PrescriptionItem>();
            CreateMap<Consultation, ConsultationSessionDTO>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));
        }

    }
}
