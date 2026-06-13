using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Domain.Entities.ConsultationModule.Enums
{
    public enum ConsultationStatus
    {
        Scheduled,
        //WaitingDoctor,
        //WaitingPatient,
        InProgress,
        Completed,
        Cancelled,
        Missed
    }
}
