using AutoMapper;
using Health.Domain.Contracts;
using Health.Domain.Entities.DoctorModule;
using Health.Domain.Entities.IdentityModule;
using Health.Services.Abstraction.AdminModule;
using Health.Services.Aggregates;
using Health.Services.Specifications.DoctorAdminSpecification;
using Health.Services.Specifications.DoctorSpecification;
using Health.Shared;
using Health.Shared.CommonResponses;
using Health.Shared.DTOs.AdminDTOs;
using Health.Shared.DTOs.IdentityDTOs;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Services.ServicesImplementation.AdminModule
{
    public class AdminService : IAdminService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminService(IUnitOfWork unitOfWork, IMapper mapper, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        #region Doctors

        public async Task<DoctorAnalyticsDataDTO> GetDoctorsAnalyticsAsync()
        {
            var repo = _unitOfWork.GetRepository<DoctorProfile, int>();

            var totalSpec = new DoctorAdminWithCountSpecification(new DoctorAdminSpecParams());
            var totalDoctors = await repo.CountAsync(totalSpec);

            var activeSpec = new DoctorAdminWithCountSpecification(new DoctorAdminSpecParams
            {
                VerificationStatus = DoctorVerificationStatus.Approved
            });
            var activeDoctors = await repo.CountAsync(activeSpec);

            var pendingSpec = new DoctorAdminWithCountSpecification(new DoctorAdminSpecParams
            {
                VerificationStatus = DoctorVerificationStatus.Pending
            });
            var pendingDoctors = await repo.CountAsync(pendingSpec);

            var doctors = await repo.GetAllAsync(totalSpec);

            decimal avgRating = 0;

            if (doctors.Any())
            {
                var totalRating = doctors.Sum(d => d.Rating);
                avgRating = totalRating / doctors.Count();
            }

            return new DoctorAnalyticsDataDTO()
            {
                TotalDoctors = totalDoctors,
                ActiveDoctors = activeDoctors,
                PendingDoctors = pendingDoctors,
                DoctorsAvgRating = avgRating
            };
        }

        public async Task<PaginatedResult<GetAllDoctorsDTO>> GetAllDoctorsAsync(DoctorAdminSpecParams queryParams)
        {
            var doctorRepo = _unitOfWork.GetRepository<DoctorProfile, int>();

            var spec = new DoctorAdminWithScheduleAndGeneratedSlots(queryParams);
            var doctors = await doctorRepo.GetAllAsync(spec);
            var mappedDoctor = _mapper.Map<IEnumerable<GetAllDoctorsDTO>>(doctors);
            var countOfMappedDoctors = mappedDoctor.Count();

            var countSpec = new DoctorAdminWithCountSpecification(queryParams);
            var countOverAll = await doctorRepo.CountAsync(countSpec);

            return new PaginatedResult<GetAllDoctorsDTO>(queryParams.PageIndex, countOfMappedDoctors, countOverAll, mappedDoctor);
        }

        public async Task<GetDoctorDetailsDTO> GetDoctorDetailsAsync(int doctorId)
        {
            var profileData = await _unitOfWork.GetRepository<DoctorProfile, int>().GetByIdAsync(doctorId);

            var userData = await _userManager.FindByIdAsync(profileData!.UserId);

            var doctorDetails = new DoctorAggregate()
            {
                ApplicationUser = userData!,
                DoctorProfile = profileData,
            };

            var mappedDoctor = _mapper.Map<GetDoctorDetailsDTO>(doctorDetails);
            mappedDoctor.ApprovedByName = await GetApprovedUserName(mappedDoctor.ApprovedBy ?? "");

            return mappedDoctor;
        }

        public async Task<bool> ApproveDoctorAsync(int id, string approvedByUserId)
        {
            var doctorProfile = await _unitOfWork
                .GetRepository<DoctorProfile, int>()
                .GetByIdAsync(id);

            if (doctorProfile == null)
                return false;

            doctorProfile.ApprovedBy = approvedByUserId;
            doctorProfile.ApprovedAt = DateTime.UtcNow;
            doctorProfile.VerificationStatus = VerificationStatus.Approved;

            var result = await _unitOfWork.SaveChanges() > 0;

            return result;
        }

        public async Task<bool> RejectDoctorAsync(int id)
        {
            var doctorProfile = await _unitOfWork
                .GetRepository<DoctorProfile, int>()
                .GetByIdAsync(id);

            if (doctorProfile == null)
                return false;

            doctorProfile.ApprovedBy = null;
            doctorProfile.ApprovedAt = null;
            doctorProfile.VerificationStatus = VerificationStatus.Rejected;

            return await _unitOfWork.SaveChanges() > 0;
        }

        public async Task<bool> RevokeDoctorAsync(int id)
        {
            var doctorProfile = await _unitOfWork
                .GetRepository<DoctorProfile, int>()
                .GetByIdAsync(id);

            if (doctorProfile == null)
                return false;

            doctorProfile.ApprovedBy = null;
            doctorProfile.ApprovedAt = null;
            doctorProfile.VerificationStatus = VerificationStatus.Suspended;

            return await _unitOfWork.SaveChanges() > 0;
        }

        #endregion

        #region Admin Management

        public async Task<ICollection<AdminDataDTO>> GetAllAdminsAsync()
        {
            var admins = await _userManager.GetUsersInRoleAsync("Admin");
            var superAdmins = await _userManager.GetUsersInRoleAsync("SuperAdmin");

            var users = admins.Concat(superAdmins).Distinct().ToList();

            var result = new List<AdminDataDTO>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                result.Add(new AdminDataDTO
                {
                    Id = user.Id,
                    Email = user.Email!,
                    FullName = $"{user.FirstName} {user.LastName}",
                    Role = roles.First(),
                    CreatedAt = user.CreatedAt.ToString(),
                    Phone = user.PhoneNumber!,
                    UserName = user.UserName!
                });
            }

            return result;
        }

        public async Task<bool> CreateAdminAsync(CreateAdminDTO dto)
        {
            var user = new ApplicationUser()
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                PhoneNumber = dto.Phone,
                CreatedAt = DateTime.UtcNow,
                UserName = $"{dto.FirstName}_{dto.LastName}"
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                return false;

            if (!await _roleManager.RoleExistsAsync(dto.Role))
                await _roleManager.CreateAsync(new IdentityRole(dto.Role));

            await _userManager.AddToRoleAsync(user, dto.Role);

            return true;

        }

        public async Task<AdminDataDTO> GetAdminById(string Id)
        {
            var user = await _userManager.FindByIdAsync(Id);

            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.First();

            var admin = new AdminDataDTO()
            {
                Id = user.Id,
                Email = user.Email,
                FullName = $"{user.FirstName} {user.LastName}",
                UserName = $"{user.FirstName}_{user.LastName}",
                Phone = user.PhoneNumber,
                CreatedAt = user.CreatedAt.ToString(),
                Role = role
            };

            return admin;
        }

        public async Task<bool> UpdateAdminAsync(UpdateAdminDTO dto, string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user is null)
                return false;

            if (!string.IsNullOrWhiteSpace(dto.FirstName) && !string.IsNullOrWhiteSpace(dto.LastName))
            {
                user.FirstName = dto.FirstName;
                user.LastName = dto.LastName;
                user.UserName = $"{dto.FirstName}_{dto.LastName}";
            }

            if (!string.IsNullOrWhiteSpace(dto.Email))
                user.Email = dto.Email;

            if (!string.IsNullOrWhiteSpace(dto.Phone))
                user.PhoneNumber = dto.Phone;

            var updateResult = await _userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
                return false;

            if (!string.IsNullOrWhiteSpace(dto.Role))
            {
                var currentRoles = await _userManager.GetRolesAsync(user);

                if (!currentRoles.Contains(dto.Role))
                {
                    var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
                    if (!removeResult.Succeeded)
                        return false;

                    var addResult = await _userManager.AddToRoleAsync(user, dto.Role);
                    if (!addResult.Succeeded)
                        return false;
                }
            }

            return true;
        }

        public async Task<bool> DeleteAdminAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
                return false;

            await _userManager.DeleteAsync(user);

            return true;
        }

        public async Task<bool> ChangePasswordAsync(string userId, ChangeAdminPasswordDTO dto)
        {
            var user = await _userManager.FindByIdAsync(userId);

            var result = await _userManager.ChangePasswordAsync(
                user,
                dto.CurrentPassword,
                dto.NewPassword);

            return result.Succeeded;
        }

        public async Task<ICollection<string>> GetAllRolesAsync()
        {
            return _roleManager.Roles.Select(r => r.Name).ToList()!;
        }

        public async Task<bool> RoleExistsAsync(string roleName)
        {
            return await _roleManager.RoleExistsAsync(roleName);
        }

        public async Task<bool> CreateRoleAsync(string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
                return false;

            var isRoleExist = await RoleExistsAsync(roleName);

            if (isRoleExist)
                return false;

            var result = await _roleManager.CreateAsync(new IdentityRole(roleName));

            return result.Succeeded;
        }
       
        public async Task<bool> DeleteRoleAsync(string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
                return false;

            var role = await _roleManager.FindByNameAsync(roleName);

            if (role == null)
                return false;

            var usersInRole = await _userManager.GetUsersInRoleAsync(roleName);

            if (usersInRole.Any())
                return false;

            var result = await _roleManager.DeleteAsync(role);

            return result.Succeeded;
        }
        
        public async Task<bool> AssignRoleToUserAsync(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return false;

            if (!await _roleManager.RoleExistsAsync(roleName))
                return false;

            var result = await _userManager.AddToRoleAsync(user, roleName);

            return result.Succeeded;
        }

        public async Task<bool> RemoveRoleFromUserAsync(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return false;

            var result = await _userManager.RemoveFromRoleAsync(user, roleName);

            return result.Succeeded;
        }

        public async Task<ICollection<string>> GetUserRolesAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return new List<string>();

            var roles = await _userManager.GetRolesAsync(user);

            return roles.ToList();
        }

        #endregion

        #region Helper Methods
        private async Task<string> GetApprovedUserName(string id)
        {
            if (string.IsNullOrEmpty(id))
                return "";

            var user = await _userManager.FindByIdAsync(id);

            if (user is null)
                return "";

            var displayName = $"{user.FirstName} {user.LastName}";

            return displayName;
        }
        #endregion

    }
}
