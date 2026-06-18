using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Shared.DTOs.PatientDashboardDTOs
{
    public class AddressResponce
    {
        public string City { get; set; } = default!;
        public string Street { get; set; } = default!;
        public string Country { get; set; } = default!;
    }
}
