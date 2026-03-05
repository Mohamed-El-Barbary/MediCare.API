using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Domain.Entities.IdentityModule
{
    public class CloudinarySettings
    {
        public string CloudName { get; set; } = null!;
        public string ApiKey { get; set; } = null!;
        public string ApiSecret { get; set; } = null!;
    }
}
