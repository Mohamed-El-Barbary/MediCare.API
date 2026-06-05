using AutoMapper;
using Health.Domain.Contracts;
using Health.Domain.Entities.AppointmentModule;
using Health.Domain.Entities.DoctorModule;
using Health.Domain.Entities.PatientModule;
using Health.Services.Abstraction.AppointmentInterface;
using Health.Services.Specifications.AppointmentSpecification;
using Health.Services.Specifications.DoctorSpecification;
using Health.Services.Specifications.PatientSpecification;
using Health.Shared;
using Health.Shared.CommonResponses;
using Health.Shared.DTOs.AppointmentDTOs;
using Health.Shared.DTOs.DoctorDTOs;
using Health.Shared.ParamsForFilterationPatientAppointment;
using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Services.ServicesImplementation.AppointmentService
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AppointmentService(IUnitOfWork unitOfWork , IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<Result<DoctorAppointmentDTO>> BookAppointmentAsync(CreateAppointmentDTO createAppointmentDTO, string userPatientId)
        {
            // Get PatientId [Identity]
            var spec = new PatientByIdWithoutIncludes(userPatientId);
            var patient = await _unitOfWork.GetRepository<PatientProfile, int>().GetByIdAsync(spec);
            if (patient is null)
                return Result<DoctorAppointmentDTO>.Fail(Error.NotFound("Patient.NotFound", "Patient Not Found"));
            int patientId = patient.Id;

            // Slots Exist
            var slot = await _unitOfWork.GetRepository<DoctorGeneratedSlots , int>().GetByIdAsync(createAppointmentDTO.DoctorGeneratedSlotsId);
            if (slot is null)
                return Result<DoctorAppointmentDTO>.Fail(Error.NotFound("Slot.NotFound", $"Slot With {createAppointmentDTO.DoctorGeneratedSlotsId} Is Not Found"));

            // Slot Availaable
            if(slot.Status != SlotStatus.Available)
                return Result<DoctorAppointmentDTO>.Fail(Error.NotFound("Slot.NotAvailable", $"Slot With {createAppointmentDTO.DoctorGeneratedSlotsId} Is Not Available"));

            // Doctor Exist
            var doctor = await _unitOfWork.GetRepository<DoctorProfile, int>().GetByIdAsync(createAppointmentDTO.DoctorProfileId);
            if (doctor is null)
                return Result<DoctorAppointmentDTO>.Fail(Error.NotFound("Doctor.NotFound", $"Doctor Is Not Found"));

            // Create Appointment
            var appointment = _mapper.Map<Appointment>(createAppointmentDTO);
            appointment.PatientProfileId = patientId;

            // Lock Slot
            slot.Status = SlotStatus.Booked;

            // hit Datebase
            await _unitOfWork.GetRepository<Appointment , int>().AddAsync(appointment);

            // Save Changes
            bool result = await _unitOfWork.SaveChanges() > 0;

            if (!result)
                return Result<DoctorAppointmentDTO>.Fail(Error.Failure("Something Wrong Happen When Adding Appointment"));

            var appointmentDto = _mapper.Map<DoctorAppointmentDTO>(appointment);
            return Result<DoctorAppointmentDTO>.Ok(appointmentDto);
        }
        public async Task<PaginatedResult<PatientAppointmentDTO>> GetPatientAppointment(string PatientUserId , AppointmentSpecParams specParams)
        {
            // Get PatientId
            var spec = new PatientByIdWithoutIncludes(PatientUserId);
            var patient = await _unitOfWork.GetRepository<PatientProfile, int>().GetByIdAsync(spec);

            if(patient is null)
                throw new Exception("Patient not found");

            var patientId = patient.Id;

            // CountOfResult
            var countSpec = new PatientAppointmentsCountSpec(patientId, specParams);
            var totalCount = await _unitOfWork .GetRepository<Appointment, int>().CountAsync(countSpec);

            // Data
            var FilterAppointment = new PatientFilteration(patientId , specParams);
            var PatientAppointments = await _unitOfWork.GetRepository<Appointment, int>().GetAllAsync(FilterAppointment);

            if (PatientAppointments is null)
                throw new Exception("PatientAppointment not found");

            // CountOfReturnedData
            int CountOfResultData = PatientAppointments.Count();
            var result = _mapper.Map<IEnumerable<PatientAppointmentDTO>>(PatientAppointments);
            return new PaginatedResult<PatientAppointmentDTO>(specParams.PageIndex, CountOfResultData, totalCount, result);
        }
        public async Task<PaginatedResult<DoctorAppointmentDTO>> GetDoctorAppointment(string DoctorUserId, AppointmentSpecParams specParams)
        {

            // Get DoctorId
            var spec = new DoctorByIdSpecification(DoctorUserId);
            var Doctor = await _unitOfWork.GetRepository<DoctorProfile, int>().GetByIdAsync(spec);

            if (Doctor is null)
                throw new Exception("Patient not found");

            var doctorId = Doctor.Id;

            // CountOfResult
            var countSpec = new DoctorAppointmentsCountSpec(doctorId, specParams);
            var totalCount = await _unitOfWork.GetRepository<Appointment, int>().CountAsync(countSpec);

            // Data
            var FilterAppointment = new DoctorAppointmentFilteration(doctorId, specParams);
            var DoctorAppointments = await _unitOfWork.GetRepository<Appointment, int>().GetAllAsync(FilterAppointment);

            if (DoctorAppointments is null)
                throw new Exception("PatientAppointment not found");

            // CountOfReturnedData
            int CountOfResultData = DoctorAppointments.Count();
            var result = _mapper.Map<IEnumerable<DoctorAppointmentDTO>>(DoctorAppointments);
            return new PaginatedResult<DoctorAppointmentDTO>(specParams.PageIndex, CountOfResultData, totalCount, result);
        }
        public async Task<Result<PatientAppointmentDTO>> GetPatientAppointmentForSpacificDoctor(int AppointmentId)
        {
            var spec = new AppointmentSpecPatientViewer(AppointmentId);
            var appointment = await _unitOfWork.GetRepository<Appointment, int>().GetByIdAsync(spec);
            if (appointment is null)
                return Error.NotFound("Appointment.NotFound", $"Appointment With This Id:{AppointmentId} Is Not Found");
            return _mapper.Map<PatientAppointmentDTO>(appointment);
        }
        public async Task<Result<DoctorAppointmentDTO>> GetDoctorAppointmentForSpacificPatient(int AppointmentId)
        {
            var spec = new AppointmentSpecDoctorViewer(AppointmentId);
            var appointment = await _unitOfWork.GetRepository<Appointment, int>().GetByIdAsync(spec);
            if (appointment is null)
                return Error.NotFound("Appointment.NotFound", $"Appointment With This Id:{AppointmentId} Is Not Found");
            return _mapper.Map<DoctorAppointmentDTO>(appointment);
        }
        public async Task<Result> CancelAppointmentAsync(int AppointmentId, string userId, string role)
        {
            var appointment = await GetAppointmentWithSlots(AppointmentId);
            if (appointment is null)
                return Result.Fail(Error.NotFound("Appointment.NotFound", $"Apointment With {AppointmentId} Not Found"));

            var validationResult = await ValidateUserAccess(appointment, userId, role);

            if (!validationResult.IsSuccess)
                return validationResult;

            var statusResult = ValidateStatus(appointment);
            if(!statusResult.IsSuccess)
                return statusResult;

            var timeResult = ValidateTime(appointment);
            if (!timeResult.IsSuccess)
                return timeResult;

            ApplyCancellation(appointment);

            await _unitOfWork.SaveChanges();

            return Result.Ok();
        }
        public async Task<Result> ConfirmAppointment(int appointmentId, string userId)
        {
            var appointment = await GetAppointmentWithSlots(appointmentId);
            if (appointment is null)
                return Result.Fail(Error.NotFound("Appointment.NotFound", $"Apointment With {appointmentId} Not Found"));

            var spec = new DoctorByIdSpecification(userId);
            var doctor = await _unitOfWork.GetRepository<DoctorProfile, int>().GetByIdAsync(spec);

            if (doctor is null)
                return Result.Fail(Error.NotFound("Doctor.NotFound", "Doctor Is Not Found"));
            var doctorId = doctor.Id;

            if(appointment.DoctorProfileId != doctorId)
                return Result.Fail(Error.Unauthorized( "Doctor.UnAuthorize" , "You are not allowed"));

            if (appointment.Status != AppointmentStatus.Pending)
                return Result.Fail(Error.Failure("Status.Failure" , "Only pending appointments can be confirmed"));

            var slot = appointment.DoctorGeneratedSlots;
            var appointmentTime = slot.SlotDate.Date + slot.StartTime;
            if (appointmentTime < DateTime.UtcNow)
                return Result.Fail(Error.Failure("AppointmentTime.Failure" , "Cannot confirm past appointment"));

            appointment.Status = AppointmentStatus.AppointmentConfirmed; 

            _unitOfWork.GetRepository<Appointment , int>().Update(appointment);

            var result = await _unitOfWork.SaveChanges();

            if (result <= 0)
                return Result.Fail(Error.Failure("confirmed.Failure" , "Failed to confirm appointment"));

            return Result.Ok();
        }
        public async Task<Result> CompleteAppointment(int appointmentId, string userId)
        {
            var appointment = await GetAppointmentWithSlots(appointmentId);
            if (appointment is null)
                return Result.Fail(Error.NotFound("Appointment.NotFound", $"Apointment With {appointmentId} Not Found"));

            var spec = new DoctorByIdSpecification(userId);
            var doctor = await _unitOfWork.GetRepository<DoctorProfile, int>().GetByIdAsync(spec);

            if (doctor is null)
                return Result.Fail(Error.NotFound("Doctor.NotFound", "Doctor Is Not Found"));
            var doctorId = doctor.Id;

            if (appointment.DoctorProfileId != doctorId)
                return Result.Fail(Error.Unauthorized("Doctor.UnAuthorize", "You are not allowed"));

            if (appointment.Status != AppointmentStatus.AppointmentConfirmed)
                return Result.Fail(Error.Failure("Status.Failure", "Only pending appointments can be confirmed"));

            var slot = appointment.DoctorGeneratedSlots;
            var appointmentTime = slot.SlotDate.Date + slot.EndTime;

            if (appointmentTime > DateTime.UtcNow)
                return Result.Fail(Error.Failure("Appointment time has not finished yet"));

            appointment.Status = AppointmentStatus.AppointmentCompleted;

            _unitOfWork.GetRepository<Appointment, int>().Update(appointment);

            var result = await _unitOfWork.SaveChanges();

            if (result <= 0)
                return Result.Fail(Error.Failure("StatusComplete.Failure", "Failed to Complete appointment"));

            return Result.Ok();
        }

        #region Helper Method For CancelAppointment
        private async Task<Appointment?> GetAppointmentWithSlots(int AppointmentId)
        {
            var spec = new GetAppointmentBySlotsSpec(AppointmentId);
            var appointment = await _unitOfWork.GetRepository<Appointment, int>().GetByIdAsync(spec);
            return appointment;
        }
        private async Task<Result> ValidateUserAccess(Appointment appointment, string userId, string role)
        {
            if (role == "Patient")
            {
                var spec = new PatientByIdSpecification(userId);
                var patient = await _unitOfWork.GetRepository<PatientProfile, int>().GetByIdAsync(spec);
                if (patient is null)
                    return Result.Fail(Error.NotFound("Patient.NotFound", "This Patient Is Not Found"));

                var patientId = patient.Id;
                if (appointment.PatientProfileId != patientId)
                    return Result.Fail(Error.Unauthorized("Patient.NotAllowed", "This Patient Is Not Allowed"));
            }
            else if (role == "Doctor")
            {
                var spec = new DoctorByIdSpecification(userId);
                var doctor = await _unitOfWork.GetRepository<DoctorProfile, int>().GetByIdAsync(spec);
                if (doctor is null)
                    return Result.Fail(Error.NotFound("Patient.NotFound", "This Patient Is Not Found"));
                var doctorId = doctor.Id;
                if (appointment.PatientProfileId != doctorId)
                    return Result.Fail(Error.Unauthorized("Patient.NotAllowed", "This Patient Is Not Allowed"));
            }

            return Result.Ok();
        }
        private Result ValidateStatus(Appointment appointment)
        {
            if (appointment.Status == AppointmentStatus.AppointmentCompleted ||
                appointment.Status == AppointmentStatus.AppointmentCancelled)
            {
                return Result.Fail(Error.Failure("Canced.Failuer", "Cannot cancel this appointment"));
            }

            return Result.Ok();
        }
        private Result ValidateTime(Appointment appointment)
        {
            var slot = appointment.DoctorGeneratedSlots;
            var appointmentTime = slot.SlotDate.Date + slot.StartTime;

            if (appointmentTime <= DateTime.UtcNow.AddHours(2))
                return Result.Fail(Error.Failure("Cancel.Failure", "Too late to cancel"));

            return Result.Ok();
        }
        private void ApplyCancellation(Appointment appointment)
        {
            appointment.Status = AppointmentStatus.AppointmentCancelled;
            var slot = appointment.DoctorGeneratedSlots;
            slot.Status = SlotStatus.Available;
        }

        #endregion

    }
}
