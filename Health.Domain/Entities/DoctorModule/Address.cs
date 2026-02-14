using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Domain.Entities.DoctorModule
{
    public class Address
    {
        public string City { get; set; } = default!;
        public string Street { get; set; } = default!;
        public string Country { get; set; } = default!;
    }
}
