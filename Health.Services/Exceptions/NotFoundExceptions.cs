using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Services.Exceptions
{
    public abstract class NotFoundExceptions(string message) :Exception(message)
    {
    }

    public sealed class DoctorNotFoundExceptions(int id) : NotFoundExceptions($"Doctor With {id} Not Found");
}
