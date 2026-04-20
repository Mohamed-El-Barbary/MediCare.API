using Health.Domain.Entities.IdentityModule;
using Health.Services.Abstraction.AdminModule;
using Health.Shared;
using Health.Shared.DTOs.AdminDTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AdminDashboard.Controllers
{
    public class DoctorsController : Controller
    {
        private readonly IAdminService _adminService;
        private readonly UserManager<ApplicationUser> _userManager;

        public DoctorsController(IAdminService adminService, UserManager<ApplicationUser> userManager)
        {
            _adminService = adminService;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index([FromQuery] DoctorAdminSpecParams queryParams)
        {
            var data = await _adminService.GetAllDoctorsAsync(queryParams);
            var analytics = await _adminService.GetDoctorsAnalyticsAsync();
            var dashboardData = new DoctorDashboardDTO
            {
                DoctorsData = data,
                Analytics = analytics,
                QueryParams = queryParams

            };
            return View(dashboardData);
        }

        public async Task<IActionResult> DoctorDetails(int id)
        {
            if (id <= 0)
            {
                TempData["ErrorMessage"] = "Invalid doctor ID. Please provide a valid identifier.";
                return RedirectToAction(nameof(Index));
            }

            var data = await _adminService.GetDoctorDetailsAsync(id);

            if (data == null)
            {
                TempData["ErrorMessage"] = $"No member found with ID: {id}. Please check and try again.";
                return RedirectToAction(nameof(Index));
            }

            return View(data);
        }

        public async Task<IActionResult> ApproveDoctor(int id, string? returnUrl = null)
        {
            if (id <= 0)
            {
                TempData["ErrorMessage"] = "Invalid doctor ID";
                return RedirectToLocal(returnUrl);
            }

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                TempData["ErrorMessage"] = "You must be logged in";
                return RedirectToLocal(returnUrl);
            }

            var result = await _adminService.ApproveDoctorAsync(id, user.Id);

            if (result)
                TempData["SuccessMessage"] = "Doctor approved successfully ✅";
            else
                TempData["ErrorMessage"] = "Failed to approve doctor ❌";

            return RedirectToLocal(returnUrl);
        }

        public async Task<IActionResult> RejectDoctor(int id, string? returnUrl = null)
        {
            if (id <= 0)
            {
                TempData["ErrorMessage"] = "Invalid doctor ID";
                return RedirectToLocal(returnUrl);
            }

            var result = await _adminService.RejectDoctorAsync(id);

            if (result)
                TempData["SuccessMessage"] = "Doctor rejected successfully ✅";
            else
                TempData["ErrorMessage"] = "Failed to reject doctor ❌";

            return RedirectToLocal(returnUrl);
        }

        public async Task<IActionResult> RevokeDoctor(int id, string? returnUrl = null)
        {
            if (id <= 0)
            {
                TempData["ErrorMessage"] = "Invalid doctor ID";
                return RedirectToLocal(returnUrl);
            }

            var result = await _adminService.RevokeDoctorAsync(id);

            if (result)
                TempData["SuccessMessage"] = "Doctor revoked successfully ✅";
            else
                TempData["ErrorMessage"] = "Failed to revoke doctor ❌";

            return RedirectToLocal(returnUrl);
        }

        private IActionResult RedirectToLocal(string? returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction(nameof(Index));
        }
    }
}
