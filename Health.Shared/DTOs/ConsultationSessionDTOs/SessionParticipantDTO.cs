using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Shared.DTOs.ConsultationSessionDTOs
{
    public class SessionParticipantDTO
    {
        public int UserId { get; set; }
        public string? Name { get; set; }
        public string? Role { get; set; }
        public bool IsConnected { get; set; }
        public DateTime? ConnectedAt { get; set; }
    }
}
