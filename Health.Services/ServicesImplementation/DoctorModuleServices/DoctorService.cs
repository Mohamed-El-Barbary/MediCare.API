using AutoMapper;
using AutoMapper.Execution;
using Health.Domain.Contracts;
using Health.Domain.Entities.DoctorModule;
using Health.Domain.Entities.IdentityModule;
using Health.Services.Abstraction.DoctorModulesAbstractions;
using Health.Services.Aggregates;
using Health.Services.Specifications.DoctorSpecification;
using Health.Shared;
using Health.Shared.CommonResponses;
using Health.Shared.DTOs.DoctorDTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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

        public Task<PaginatedResult<DoctorDTO>> GetAllDoctorsAsync(DoctorSpecParams QueryParams)
        {
            throw new NotImplementedException();
        }

        public async Task<Result<DoctorDTO>> GetDoctorByIdAsync(int id)
        {
            var spec = new DoctorWithScheduleAndGeneratedSlots(id);
            var doctor = await _unitOfWork.GetRepository<DoctorProfile , int>().GetByIdAsync(spec);
            if (doctor is null)
            {
                return Error.NotFound("Doctor.NotFound", $"Doctor With Id:{id} Is Not Found");
            }

            return _mapper.Map<DoctorProfile ,DoctorDTO>(doctor);
        }
    }
}
