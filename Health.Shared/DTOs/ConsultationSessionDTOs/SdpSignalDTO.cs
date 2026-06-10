using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Shared.DTOs.ConsultationSessionDTOs
{
    public class SdpSignalDTO
    {
        public string Type { get; set; } = default!;
        public string Sdp { get; set; } = default!;
    }
}
