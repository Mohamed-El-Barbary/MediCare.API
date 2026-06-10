using Health.Shared.CommonResponses;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Health.Services.Abstraction.IdentityModule
{
    public interface IProfileCompletionService
    {
        Task<Result<int>> CalculateAndUpdateDoctorStatusAsync(string doctorId);

    }
}
