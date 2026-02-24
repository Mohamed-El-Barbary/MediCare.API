using Health.Domain.Entities.DoctorModule;
using Health.Domain.Entities.IdentityModule;
using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;

namespace Health.Services.Aggregates
{
    public class DoctorAggregate
    {
        public ApplicationUser ApplicationUser { get; set; } = default!;

        public DoctorProfile DoctorProfile { get; set; } = default!;
    }
}
