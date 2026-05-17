using AutoMapper;
using Health.Domain.Entities.ReviewModule;
using Health.Shared.DTOs.ReviewDTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Services.MappingProfiles.ReviewMapping
{
    public class ReviewMappingProfile : Profile
    {
        public ReviewMappingProfile()
        {
            CreateMap<CreateReviewDto, Review>()
                    .ForMember(desc => desc.PatientId, opt => opt.Ignore())
                    .ForMember(desc => desc.doctorId, opt => opt.Ignore());

            CreateMap<Review, ReviewResposeForPatinetDTO>()
                    .ForMember(desc => desc.DoctorName , opt => opt.MapFrom(src => src.doctor.DisplayName));

        }
    }
}
