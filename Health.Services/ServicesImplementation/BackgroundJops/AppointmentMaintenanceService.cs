using Health.Domain.Contracts;
using Health.Domain.Entities.DoctorModule;
using Health.Services.Abstraction.BackgroundJop;
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
        public async Task ExpireSlotsAsync()
        {
            var data = DateTime.Now;

            var expireSlotsSpec = new ExpiredSlotsSpec();
            var expireSlots = await _unitOfWork.GetRepository<DoctorGeneratedSlots, int>().GetAllAsync(expireSlotsSpec);

            foreach (var slot in expireSlots)
            {
                slot.Status = SlotStatus.Expired;
            }

            await _unitOfWork.SaveChanges();
        }
    }
}
