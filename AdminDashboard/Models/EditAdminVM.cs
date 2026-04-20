using Health.Shared.DTOs.AdminDTOs;

namespace AdminDashboard.Models
{
    public class EditAdminVM
    {
        public UpdateAdminDTO Admin { get; set; } = default!;

        public ICollection<string>? AssignedRoles { get; set; }
        public ICollection<string>? AllRoles { get; set; }
    }
}
