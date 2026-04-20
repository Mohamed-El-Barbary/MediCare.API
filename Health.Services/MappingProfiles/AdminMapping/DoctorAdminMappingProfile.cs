using AutoMapper;
using Health.Domain.Entities.DoctorModule;
using Health.Services.Aggregates;
using Health.Shared.DTOs.AdminDTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Services.MappingProfiles.AdminMapping
{
    internal class DoctorAdminMappingProfile : Profile
    {
        public DoctorAdminMappingProfile()
        {
            CreateMap<DoctorProfile, GetAllDoctorsDTO>();
            CreateMap<DoctorAggregate, GetDoctorDetailsDTO>()
            // Identity
            .ForMember(d => d.Email, o => o.MapFrom(s => s.ApplicationUser.Email))
            .ForMember(d => d.PhoneNumber, o => o.MapFrom(s => s.ApplicationUser.PhoneNumber))
            // Doctor Profile
            .ForMember(d => d.Id, o => o.MapFrom(s => s.DoctorProfile.Id))
            .ForMember(d => d.JoinDate, o => o.MapFrom(s => s.DoctorProfile.JoinDate))
            .ForMember(d => d.FullName, o => o.MapFrom(s => s.DoctorProfile.DisplayName))
            .ForMember(d => d.PhoneClinc, o => o.MapFrom(s => s.DoctorProfile.PhoneClinc))
            .ForMember(d => d.Gender, o => o.MapFrom(s => s.DoctorProfile.Gender.ToString()))
            .ForMember(d => d.Specialization, o => o.MapFrom(s => s.DoctorProfile.Specialization))
            .ForMember(d => d.ExperienceYears, o => o.MapFrom(s => s.DoctorProfile.YearsOfExperience))
            .ForMember(d => d.PriceConsultation, o => o.MapFrom(s => s.DoctorProfile.PriceConsultation))
            .ForMember(d => d.Rating, o => o.MapFrom(s => s.DoctorProfile.Rating))
            .ForMember(d => d.ClinicLocation, o => o.MapFrom(s => s.DoctorProfile.ClinicLocation))
            .ForMember(d => d.Address, o => o.MapFrom(s =>
                $"{s.DoctorProfile.Address.Street}, {s.DoctorProfile.Address.City}, {s.DoctorProfile.Address.Country}"))
            .ForMember(d => d.Bio, o => o.MapFrom(s => s.DoctorProfile.Bio))
            .ForMember(d => d.SyndicateCardUrl, o => o.MapFrom(s => s.DoctorProfile.SyndicateCardUrl))
            // Status
            .ForMember(d => d.Status, o => o.MapFrom(s => s.DoctorProfile.VerificationStatus.ToString()))
            // Approved
            .ForMember(d => d.ApprovedBy, o => o.MapFrom(s => s.DoctorProfile.ApprovedBy))
            .ForMember(d => d.ApprovedAt, o => o.MapFrom(s => s.DoctorProfile.ApprovedAt))

            // chedule
            .ForMember(d => d.Schedule, o => o.MapFrom(s => s.DoctorProfile.DoctorSchedule));

            CreateMap<DoctorSchedule, DoctorSlotDTO>()
                .ForMember(d => d.Day, o => o.MapFrom(s => s.DayOfWeek))
                .ForMember(d => d.From, o => o.MapFrom(s => s.StartTime.ToString("hh:mm tt")))
                .ForMember(d => d.To, o => o.MapFrom(s => s.EndTime.ToString("hh:mm tt")));

        }
    }
}
