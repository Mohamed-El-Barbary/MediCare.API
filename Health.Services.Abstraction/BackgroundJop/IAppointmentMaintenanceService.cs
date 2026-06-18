using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Services.Abstraction.BackgroundJop
{
    public interface IAppointmentMaintenanceService
    {
        Task ExpireSlotsAsync();
        Task CancelUnpaidAppointmentsAsync();
     }

}
