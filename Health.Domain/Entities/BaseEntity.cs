using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Domain.Entities
{
    public class BaseEntity<Tkey>
    {
        public Tkey Id { get; set; } = default!;
    }
}
