using Health.Domain.Entities.ConsultationModule;
using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Services.Specifications.ConsultationSpecification
{
    public class ActiveConnectionByUserSpecification : BaseSpecification<SessionConnection, int>
    {
        public ActiveConnectionByUserSpecification(int consultationId, int userId) 
            : base(s => s.ConsultationId == consultationId && s.UserId == userId && s.IsActive)
        {

        }
    }
}
