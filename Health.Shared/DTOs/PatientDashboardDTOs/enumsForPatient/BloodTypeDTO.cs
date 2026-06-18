using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Health.Shared.DTOs.PatientDashboardDTOs.enumsForPatient
{
    public enum BloodTypeDTO
    {
        [Description("A+")]
        APositive = 1,

        [Description("A-")]
        ANegative,

        [Description("B+")]
        BPositive,

        [Description("B-")]
        BNegative,

        [Description("AB+")]
        ABPositive,

        [Description("AB-")]
        ABNegative,

        [Description("O+")]
        OPositive,

        [Description("O-")]
        ONegative
    }
}
