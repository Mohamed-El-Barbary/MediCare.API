using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Shared
{
    public class DoctorAdminSpecParams : DoctorSpecParams
    {
        public DoctorVerificationStatus? VerificationStatus { get; set; }
    }
}
