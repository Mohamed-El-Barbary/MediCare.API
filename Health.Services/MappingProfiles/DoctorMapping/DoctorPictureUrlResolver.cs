using AutoMapper;
using Health.Services.Aggregates;
using Health.Shared.DTOs.DoctorDTOs;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Services.MappingProfiles.DoctorMapping
{
    public class DoctorPictureUrlResolver : IValueResolver<DoctorAggregate, DoctorDTO, string>
    {
        private readonly IConfiguration _configuration;

        public DoctorPictureUrlResolver(IConfiguration configuration)
        {
            this._configuration = configuration;
        }
        public string Resolve(DoctorAggregate source, DoctorDTO destination, string destMember, ResolutionContext context)
        {
            if (string.IsNullOrEmpty(source.DoctorProfile.DoctorPictureUrl))
            {
                return string.Empty;
            }

            if (source.DoctorProfile.DoctorPictureUrl.StartsWith("http") || source.DoctorProfile.DoctorPictureUrl.StartsWith("https"))
            {
                return source.DoctorProfile.DoctorPictureUrl;
            }
            var BaseUrl = _configuration.GetSection("URLs")["BaseUrl"];
            var PictureUrl = $"{BaseUrl}{source.DoctorProfile.DoctorPictureUrl}";

            return PictureUrl;
        }
    }
}
