using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Domain.Entities.DoctorModule
{
    public enum VerificationStatus
    {
        PendingProfileCompletion = 0,
        PendingApproval = 1,
        Approved = 2,
        Rejected = 3,
        Suspended = 4
    }
}
