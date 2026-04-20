using Health.Shared;
using Health.Shared.CommonResponses;
using Health.Shared.DTOs.AdminDTOs;
using Health.Shared.DTOs.IdentityDTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Services.Abstraction.AdminModule
{
    public interface IAdminService
    {
        #region Doctor
        Task<PaginatedResult<GetAllDoctorsDTO>> GetAllDoctorsAsync(DoctorAdminSpecParams queryParams);
        Task<DoctorAnalyticsDataDTO> GetDoctorsAnalyticsAsync();
        Task<GetDoctorDetailsDTO> GetDoctorDetailsAsync(int doctorId);
        Task<bool> ApproveDoctorAsync(int id, string approvedByUserId);
        Task<bool> RejectDoctorAsync(int id);
        Task<bool> RevokeDoctorAsync(int id);
        #endregion

        #region Admin Management
        Task<ICollection<AdminDataDTO>> GetAllAdminsAsync();
        Task<AdminDataDTO> GetAdminById(string Id);
        Task<bool> CreateAdminAsync(CreateAdminDTO dto);
        Task<bool> UpdateAdminAsync(UpdateAdminDTO dto, string id);
        Task<bool> DeleteAdminAsync(string id);
        Task<bool> ChangePasswordAsync(string userId, ChangeAdminPasswordDTO dto);
        Task<ICollection<string>> GetAllRolesAsync();
        Task<bool> CreateRoleAsync(string roleName);
        Task<bool> DeleteRoleAsync(string roleName);
        Task<bool> RoleExistsAsync(string roleName);
        Task<bool> AssignRoleToUserAsync(string userId, string roleName);
        Task<bool> RemoveRoleFromUserAsync(string userId, string roleName);
        Task<ICollection<string>> GetUserRolesAsync(string userId);
        #endregion
    }
}
