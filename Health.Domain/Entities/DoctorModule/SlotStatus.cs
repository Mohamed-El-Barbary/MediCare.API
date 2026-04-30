using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Domain.Entities.DoctorModule
{
    public enum SlotStatus
    {
        Available = 0,
        Booked = 1,
        Expired = 2,
        Blocked = 3
    }
}
