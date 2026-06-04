using Health.Domain.Contracts;
using Health.Domain.Entities.AppointmentModule;
using Health.Domain.Entities.DoctorModule;
using Health.Services.Abstraction.BackgroundJop;
using Health.Services.Specifications.AppointmentSpecification;
using Health.Services.Specifications.DoctorSpecification;
using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Services.ServicesImplementation.BackgroundJops
{
    public class AppointmentMaintenanceService : IAppointmentMaintenanceService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AppointmentMaintenanceService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task CancelUnpaidAppointmentsAsync()
        {
            var expirationTime = DateTime.UtcNow.AddHours(-24);
            var appointmentExpire = new ExpireAppointmentWithSlots(expirationTime);
            var appointments = await _unitOfWork.GetRepository<Appointment , int>().GetAllAsync(appointmentExpire);

            foreach (var appointment in appointments)
            {
                appointment.Status = AppointmentStatus.AppointmentCancelled;
                DateTime SlotDatTime = appointment.DoctorGeneratedSlots.SlotDate + appointment.DoctorGeneratedSlots.StartTime;
                if (SlotDatTime > DateTime.Now)
                {
                    appointment.DoctorGeneratedSlots.Status = SlotStatus.Available;
                }
                else
                {
                    appointment.DoctorGeneratedSlots.Status = SlotStatus.Expired;
                }
            }

            await _unitOfWork.SaveChanges();
        }

        public async Task ExpireSlotsAsync()
        {
            var availableSlotsSpec = new ExpiredSlotsSpec();
            var AvailableSlots = await _unitOfWork.GetRepository<DoctorGeneratedSlots, int>().GetAllAsync(availableSlotsSpec);
    
            foreach (var slot in AvailableSlots)
            {
                DateTime SlotDatTime = slot.SlotDate + slot.StartTime;
                if (SlotDatTime > DateTime.Now)
                {
                  slot.Status = SlotStatus.Expired;
                }
            }

            await _unitOfWork.SaveChanges();
        }



    }
}
