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
        public async Task<Result> BookAppointmentAsync(CreateAppointmentDTO createAppointmentDTO, string userPatientId)
        {
            // Get PatientId [Identity]
            var spec = new PatientByIdWithoutIncludes(userPatientId);
            var patient = await _unitOfWork.GetRepository<PatientProfile, int>().GetByIdAsync(spec);
            if (patient is null)
                return Result.Fail(Error.NotFound("Patient.NotFound", "Patient Not Found"));
            int patientId = patient.Id;

            // Slots Exist
            var slot = await _unitOfWork.GetRepository<DoctorGeneratedSlots , int>().GetByIdAsync(createAppointmentDTO.DoctorGeneratedSlotsId);
            if (slot is null)
                return Result.Fail(Error.NotFound("Slot.NotFound", $"Slot With {createAppointmentDTO.DoctorGeneratedSlotsId} Is Not Found"));

            // Slot Availaable
            if(slot.Status != SlotStatus.Available)
                return Result.Fail(Error.NotFound("Slot.NotAvailable", $"Slot With {createAppointmentDTO.DoctorGeneratedSlotsId} Is Not Available"));

            // Doctor Exist
            var doctor = await _unitOfWork.GetRepository<DoctorProfile, int>().GetByIdAsync(createAppointmentDTO.DoctorProfileId);
            if (doctor is null)
                return Result.Fail(Error.NotFound("Doctor.NotFound", $"Doctor Is Not Found"));

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
                return Result.Fail(Error.Failure("Something Wrong Happen When Adding Appointment"));

            return Result.Ok();
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

    }
}
