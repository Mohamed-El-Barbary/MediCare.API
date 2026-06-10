using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Domain.Entities.ConsultationModule
{
    public class SessionConnection : BaseEntity<int>
    {
        public int ConsultationId { get; set; }
        public Consultation Consultation { get; set; } = default!;
        public int UserId { get; set; }
        public string ConnectionId { get; set; } = default!;
        public DateTime ConnectedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
    }
}
