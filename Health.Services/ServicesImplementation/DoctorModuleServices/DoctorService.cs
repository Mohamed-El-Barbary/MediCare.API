using AutoMapper;
using AutoMapper.Execution;
using Health.Domain.Contracts;
using Health.Domain.Entities.DoctorModule;
using Health.Domain.Entities.IdentityModule;
using Health.Services.Abstraction.DoctorModulesAbstractions;
using Health.Services.Aggregates;
using Health.Services.Specifications.DoctorGeneratedSlotsSpecification;
using Health.Services.Specifications.DoctorSceduleSpecification;
using Health.Services.Specifications.DoctorSpecification;
using Health.Shared;
using Health.Shared.CommonResponses;
using Health.Shared.DTOs.DoctorDTOs;
using Health.Shared.DTOs.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Text;
using static System.Reflection.Metadata.BlobBuilder;

namespace Health.Services.ServicesImplementation.DoctorModuleServices
{
    public class DoctorService : IDoctorService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IDoctorScheduleRepository _doctorScheduleRepository;
        private readonly IDoctorGenerateSlotsRepository _slotRepo;

        public DoctorService(IUnitOfWork unitOfWork, 
                             IMapper mapper , 
                             IDoctorScheduleRepository doctorScheduleRepository , 
                             IDoctorGenerateSlotsRepository SlotRepo
                           
            )
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _doctorScheduleRepository = doctorScheduleRepository;
            _slotRepo = SlotRepo;
        }


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

            return _mapper.Map<DoctorDTO>(doctor);
        }

        public async Task<Result> AddScheduleAsync(string userDoctorId, DoctorScheduleDTO dto)
        {
            var doctorSpec = new DoctorByUserIdSpec(userDoctorId);
            // 1 Check doctor exists
            var doctor = await _unitOfWork.GetRepository<DoctorProfile, int>().GetByIdAsync(doctorSpec);

            if (doctor == null)
                return Result.Fail(Error.NotFound("Doctor Is Not Found"));

            //  Validation
            if (dto.StartTime >= dto.EndTime)
                return Result.Fail(Error.Validation("StartTime must be less than EndTime"));

            if (dto.SlotDurationMinutes <= 0)
                return  Result.Fail(Error.InvalidCredentials("Invalid slot duration"));

            var doctorId = doctor.Id;

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

        public async Task<Result<IEnumerable<DoctorSceduleToReturn>>> GetAllDoctorScheduleAsync(string userDoctorId)
        {
            // Get Doctor ID
            var doctorSpec = new DoctorByUserIdSpec(userDoctorId);
            var doctor = await _unitOfWork.GetRepository<DoctorProfile, int>().GetByIdAsync(doctorSpec);

            if (doctor == null)
                return Result<IEnumerable<DoctorSceduleToReturn>>.Fail(Error.NotFound("Doctor Is Not Found"));

            int doctorId = doctor.Id;

            var DoctorScudleElement = _unitOfWork.GetRepository<DoctorSchedule, int>();
            var spec = new DoctorSceduleByDoctorProfileIdSpec(doctorId);
            var schedules = await DoctorScudleElement.GetAllAsync(spec);

            if (!schedules.Any())
            {
                return Error.NotFound("DoctorScedule.NotFound", $"DoctorSecdule With Id:{doctorId} Is Not Found");
            }

            var schedulesDTO = _mapper.Map<IEnumerable<DoctorSceduleToReturn>>(schedules);

            return Result<IEnumerable<DoctorSceduleToReturn>>.Ok(schedulesDTO);
        }

        public async Task<Result> DeleteScheduleAsync(int scheduleId, string userDoctorId)
        {
            // Get Doctor Id [identity]
            var doctorSpec = new DoctorByUserIdSpec(userDoctorId);
            var doctor = await _unitOfWork.GetRepository<DoctorProfile, int>().GetByIdAsync(doctorSpec);

            if (doctor == null)
                return Result.Fail(Error.NotFound("Doctor Is Not Found"));

            int doctorProfileId = doctor.Id;

            var scheduleRepo = _unitOfWork.GetRepository<DoctorSchedule, int>();

            var schedule = await scheduleRepo.GetByIdAsync(scheduleId);

            if (schedule == null)
                return Result.Fail(Error.NotFound("Schedule.NotFound", "Schedule not found"));

            if (schedule.DoctorProfileId != doctorProfileId)
                return Result.Fail(Error.Failure("Scedule.NotAllowed" , "You are not allowed to delete this schedule"));


            scheduleRepo.Delete(schedule);

            await _unitOfWork.SaveChanges();

            return Result.Ok();
        }

        public async Task<Result> UpdateScheduleAsync(string userDoctorId, int scheduleId, DoctorScheduleDTO dto)
        {
            // Get doctorProfileId
            var doctorSpec = new DoctorByUserIdSpec(userDoctorId);
            var doctor = await _unitOfWork.GetRepository<DoctorProfile, int>().GetByIdAsync(doctorSpec);

            if (doctor == null)
                return Result.Fail(Error.NotFound("Doctor Is Not Found"));

            int doctorProfileId = doctor.Id;

            var scheduleRepo = _unitOfWork.GetRepository<DoctorSchedule, int>();

            var scheduleToUpdate = await scheduleRepo.GetByIdAsync(scheduleId);

            if (scheduleToUpdate == null || scheduleToUpdate.DoctorProfileId != doctorProfileId)
                return Result.Fail(Error.NotFound("Schedule not found or does not belong to this doctor"));

            
            var spec = new DoctorSceduleByDoctorProfileIdSpec(doctorProfileId, scheduleId);
            var schedules = await scheduleRepo.GetAllAsync(spec);

            
            var exists = schedules.Any(s => s.DayOfWeek == dto.DayOfWeek);
            if (exists)
                return Result.Fail(Error.Failure("This doctor already has a schedule for this day"));

            
            scheduleToUpdate.DayOfWeek = dto.DayOfWeek;
            scheduleToUpdate.StartTime = dto.StartTime;
            scheduleToUpdate.EndTime = dto.EndTime;
            scheduleToUpdate.SlotDurationMinutes = dto.SlotDurationMinutes;

            scheduleRepo.Update(scheduleToUpdate);
            await _unitOfWork.SaveChanges();

            return Result.Ok();
        }

        public async Task<Result> GenerateSlotsBySchedule(int scheduleId, GeneratedSlotsRequestDto generatedSlotsRequestDto)
        {

            if (generatedSlotsRequestDto.StartDate > generatedSlotsRequestDto.EndDate)
                return Result.Fail(Error.InvalidCredentials("Result.InvalidCredential", "StartDate must be <= EndDate"));

            // Get Scheule
            var schedule = await _unitOfWork.GetRepository<DoctorSchedule , int>().GetByIdAsync(scheduleId);
            if (schedule is null)
                return Result.Fail(Error.NotFound("Schedule.Notfound", $"Schedule With {scheduleId} Is Not Found For This Doctor"));

            //validation
            if (schedule.StartTime >= schedule.EndTime)
                return Result.Fail(Error.Failure("Generate Is Failure", "Invalid Schedule Time"));

            if(schedule.SlotDurationMinutes <= 0)
                return Result.Fail(Error.Failure("Generate Is Failure", "Invalid Slot Duration"));

            // doctorId From Doctor Schedule
            var doctorId = schedule.DoctorProfileId;

            var duration = TimeSpan.FromMinutes(schedule.SlotDurationMinutes);

            var existingSlots = await _slotRepo
                                    .GetByDoctorAndDateRange
                                    (
                                      doctorId, 
                                      generatedSlotsRequestDto.StartDate, 
                                      generatedSlotsRequestDto.EndDate
                                    );

            var existingSet = existingSlots
                                .Select(s => (s.SlotDate.Date, s.StartTime))
                                .ToHashSet();

            var newSlots = new List<DoctorGeneratedSlots>();

            // Generate Slots
            for (var date = generatedSlotsRequestDto.StartDate.Date; date <= generatedSlotsRequestDto.EndDate.Date; date = date.AddDays(1))
            {
                // check day Match
                if(date.DayOfWeek != schedule.DayOfWeek)
                    continue;

                var current = schedule.StartTime;

                while (current + duration <= schedule.EndTime)
                {
                    if (!existingSet.Contains((date, current)))
                    {
                        newSlots.Add(new DoctorGeneratedSlots
                        {
                            DoctorProfileId = doctorId,       
                            DoctorScheduleId = schedule.Id,
                            SlotDate = date,
                            StartTime = current,
                            EndTime = current.Add(duration),
                            Status = SlotStatus.Available
                        });
                    }

                    current = current.Add(duration);
                }

            }
          
            if (newSlots.Any())
            {
                await _unitOfWork.GetRepository<DoctorGeneratedSlots , int>().AddRangeAsync(newSlots);
            }

            bool result = await _unitOfWork.SaveChanges() > 0;

            if(!result)
                return Result.Fail(Error.Failure("Something Wrong Happen When Adding Slots")); 
            
            
            return Result.Ok();
        }

        public async Task<Result<IEnumerable<GeneratedSlotsDTO>>> GetDoctorSlots(string userDoctorId, DateTime? date)
        {
            // Get Doctor ID
            var doctorSpec = new DoctorByUserIdSpec(userDoctorId);
            var doctor = await _unitOfWork.GetRepository<DoctorProfile, int>().GetByIdAsync(doctorSpec);

            if (doctor == null)
                return Result<IEnumerable<GeneratedSlotsDTO>>.Fail(Error.NotFound("Doctor Is Not Found"));

            int doctorId = doctor.Id;

            var spec = new DoctorSlotsSpec(doctorId, date);
            var Slots = await _unitOfWork.GetRepository<DoctorGeneratedSlots , int>().GetAllAsync(spec);

            return Slots.Select(s => new GeneratedSlotsDTO
            {
                Id = s.Id,
                SlotDate = s.SlotDate,
                StartTime = s.StartTime,
                EndTime = s.EndTime,
                Status = (EnumSlotStatusDTO)s.Status
            }).ToList();
        }
    
    }
}
