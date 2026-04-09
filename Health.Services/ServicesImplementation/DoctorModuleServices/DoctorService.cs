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
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Services.ServicesImplementation.DoctorModuleServices
{
    public class DoctorService : IDoctorService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IDoctorScheduleRepository _doctorScheduleRepository;

        public DoctorService(IUnitOfWork unitOfWork, IMapper mapper , IDoctorScheduleRepository doctorScheduleRepository)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _doctorScheduleRepository = doctorScheduleRepository;
        }

     

          /*
              GET /api/doctors/{id}/slots
              POST /api/doctors/{id}/generate-slots
              POST /api/doctors/{id}/schedule
              PUT  /api/doctors/{id}/schedule
              GET  /api/doctors/{id}/schedule
              DELETE /api/schedule/{scheduleId}
         */
         //  POST /api/doctors/{id}/schedule

        public async Task<PaginatedResult<DoctorDTO>> GetAllDoctorsAsync(DoctorSpecParams queryParams)
        {
            var DoctorElement = _unitOfWork.GetRepository<DoctorProfile, int>();

            var spec = new DoctorWithScheduleAndGeneratedSlots(queryParams);
            var Doctors = await DoctorElement.GetAllAsync(spec);
            var DataToResult = _mapper.Map<IEnumerable<DoctorDTO>>(Doctors);
            var CountOfResultData = DataToResult.Count();

            var CountSpec = new DoctorWithCountSpecification(queryParams);
            var CountOverAll = await DoctorElement.CountAsync(CountSpec);

            return new PaginatedResult<DoctorDTO>(queryParams.PageIndex, CountOfResultData, CountOverAll, DataToResult);
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


        public async Task<Result> AddScheduleAsync(int doctorId, DoctorScheduleDTO dto)
        {
            // 1 Check doctor exists
            var doctor = await _unitOfWork.GetRepository<DoctorProfile, int>().GetByIdAsync(doctorId);

            if (doctor == null)
                return Result.Fail(Error.NotFound("Doctor Is Not Found"));

            //  Validation
            if (dto.StartTime >= dto.EndTime)
                return Result.Fail(Error.Validation("StartTime must be less than EndTime"));

            if (dto.SlotDurationMinutes <= 0)
                return  Result.Fail(Error.InvalidCredentials("Invalid slot duration"));

            // Check dublicate Day
            var exists = await _doctorScheduleRepository
                              .ExistsAsync(s => s.DoctorProfileId == doctorId
                                          && s.DayOfWeek == dto.DayOfWeek);

            if (exists)
                return Result.Fail(Error.Failure("Schedule already exists for this day"));


            //  Create Schedule
            var schedule = _mapper.Map<DoctorSchedule>(dto);
            schedule.DoctorProfileId = doctorId;

            await _unitOfWork.GetRepository<DoctorSchedule , int>().AddAsync(schedule);

            bool result = await _unitOfWork.SaveChanges() > 0;

            if (!result)
                return Result.Fail(Error.Failure("Something Wrong Happen When Adding Schedule"));

            return Result.Ok();
        }


    }
}
