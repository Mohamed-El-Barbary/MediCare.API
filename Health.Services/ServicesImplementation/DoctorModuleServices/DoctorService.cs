using AutoMapper;
using AutoMapper.Execution;
using Health.Domain.Contracts;
using Health.Domain.Entities.DoctorModule;
using Health.Domain.Entities.IdentityModule;
using Health.Services.Abstraction.DoctorModulesAbstractions;
using Health.Services.Aggregates;
using Health.Services.Specifications.DoctorSpecification;
using Health.Shared.CommonResponses;
using Health.Shared.DTOs.DoctorDTOs;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Services.ServicesImplementation.DoctorModuleServices
{
    public class DoctorService : IDoctorService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public DoctorService(IUnitOfWork unitOfWork , UserManager<ApplicationUser> userManager , IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<Result<IEnumerable<DoctorDTO>>> GetAllDoctorsAsync()
        {
            var spec = new DoctorWithScheduleAndGeneratedSlots(); 

            var doctorProfiles = await _unitOfWork
                .GetRepository<DoctorProfile, int>()
                .GetAllAsync(spec);

            if (doctorProfiles is null || !doctorProfiles.Any())
                return  Error.NotFound("Doctor.NotFound", "Doctor Is Not Found");

            var doctorDTOs = new List<DoctorDTO>();

            var users = _userManager.Users.ToList();
            var userDict = users.ToDictionary(u => u.Id);

            foreach (var doctorProfile in doctorProfiles)
            {
                if (!userDict.TryGetValue(doctorProfile.UserId, out var userDoctor))
                     return Error.NotFound("Doctor.NotFound", "There Is doctor Is Not Found");

                var aggregate = new DoctorAggregate
                {
                    ApplicationUser = userDoctor,
                    DoctorProfile = doctorProfile,
                };

                doctorDTOs.Add(_mapper.Map<DoctorAggregate, DoctorDTO>(aggregate));
            }

            return doctorDTOs;
        }


        public async Task<Result<DoctorDTO>> GetDoctorByIdAsync(int id)
        {
            var spec = new DoctorWithScheduleAndGeneratedSlots(id);
            var DoctorProfile = await _unitOfWork.GetRepository<DoctorProfile , int>().GetByIdAsync(spec);

            if (DoctorProfile is null)
                Error.NotFound("Doctor.NotFound", $"Doctor With Id:{id} Is Not Found");

            var userDoctor = await _userManager.FindByIdAsync(DoctorProfile!.UserId);

            if(userDoctor is null)
                Error.NotFound("Doctor.NotFound", $"Doctor With Id:{id} Is Not Found");

            var aggregate = new DoctorAggregate
            {
               ApplicationUser = userDoctor!,
               DoctorProfile = DoctorProfile,
            };

            return _mapper.Map<DoctorAggregate , DoctorDTO>(aggregate);
        }
    }
}
