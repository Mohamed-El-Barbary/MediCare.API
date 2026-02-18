using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Domain.Contracts
{
    public interface IDataInitializer
    {
        Task InitializeAsync();
    }
}
