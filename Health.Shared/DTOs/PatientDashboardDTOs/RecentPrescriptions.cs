using Health.Shared.DTOs.ConsultationDTOs;

namespace Health.Shared.DTOs.PatientDashboardDTOs
{
    public sealed record RecentPrescriptions
    (
        IEnumerable<PrescriptionDTO> recentPrescriptions
    );
}