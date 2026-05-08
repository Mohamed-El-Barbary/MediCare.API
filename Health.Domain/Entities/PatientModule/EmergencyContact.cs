using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Domain.Entities.PatientModule
{
    public class EmergencyContact
    {
        public string ContactName { get; set; } = default!;

        public string PhoneNumber { get; set; } = default!;
    }
}
