using Health.Domain.Entities.ConsultationModule;
using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Services.Specifications.ConsultationSpecification
{
    public class ConsultationByConnectionIdSpecification : BaseSpecification<Consultation, int>
    {
        public ConsultationByConnectionIdSpecification(string connectionId) : base(c => c.Connections.Any(sc => sc.ConnectionId == connectionId))
        {
            AddInclude(c => c.Connections);
        }
    }
}
