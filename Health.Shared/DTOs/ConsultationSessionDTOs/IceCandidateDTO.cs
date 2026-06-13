using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Shared.DTOs.ConsultationSessionDTOs
{
    public class IceCandidateDTO
    {
        public string Candidate { get; set; } = default!;
        public string SdpMid { get; set; } = default!;
        public int SdpMLineIndex { get; set; }
    }
}
