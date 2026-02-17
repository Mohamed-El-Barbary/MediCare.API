using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Shared.CommonResponses
{
    public enum ErrorTypes
    {
        Failure = 0,
        Validation,
        NotFound,
        Unauthorized,
        Forbidden,
        InvalidCredentials
    }
}
