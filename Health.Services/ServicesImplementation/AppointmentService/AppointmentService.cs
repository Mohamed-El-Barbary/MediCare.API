using AutoMapper;
using Health.Domain.Contracts;
using Health.Domain.Entities.AppointmentModule;
using Health.Domain.Entities.DoctorModule;
using Health.Services.Abstraction.AppointmentInterface;
using Health.Shared.CommonResponses;
using Health.Shared.DTOs.AppointmentDTOs;
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
        public async Task<Result> BookAppointmentAsync(CreateAppointmentDTO createAppointmentDTO, int patientId)
        {
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
    }
}
