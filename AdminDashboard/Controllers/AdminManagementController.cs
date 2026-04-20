using AdminDashboard.Models;
using Health.Services.Abstraction.AdminModule;
using Health.Shared.DTOs.AdminDTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AdminDashboard.Controllers
{
    public class AdminManagementController : Controller
    {
        private readonly IAdminService _adminService;

        public AdminManagementController(IAdminService adminService)
        {
            _adminService = adminService;
        }
        public async Task<IActionResult> Index()
        {
            var users = await _adminService.GetAllAdminsAsync();
            if (users == null)
                return ViewBag.Error = "Admins Not Fount";

            return View(users);
        }

        public ActionResult AddAdmin()
        {
            return View();
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> AddAdmin(CreateAdminDTO createAdminDTO)
        {
            if (!ModelState.IsValid)
                return View(createAdminDTO);


            var result = await _adminService.CreateAdminAsync(createAdminDTO);

            if (!result)
            {
                ModelState.AddModelError("", "Failed to create admin");
                return View(createAdminDTO);
            }

            TempData["SuccessMessage"] = "Admin Created Successfully";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> EditAdmin(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["ErrorMessage"] = $"Admin with {id} not found";
                return RedirectToAction(nameof(Index));
            }

            var admin = await _adminService.GetAdminById(id);

            if (admin == null)
            {
                TempData["ErrorMessage"] = "Admin Not Found";
                return RedirectToAction(nameof(Index));
            }

            var names = admin.FullName.Trim().Split(' ');

            var vm = new EditAdminVM
            {
                Admin = new UpdateAdminDTO
                {
                    Id = admin.Id,
                    FirstName = names.Length > 0 ? names[0] : "",
                    LastName = names.Length > 1 ? names[1] : "",
                    Email = admin.Email,
                    Phone = admin.Phone,
                    Role = admin.Role
                }
            };

            await LoadRolesAsync(vm);

            return View(vm);
        }
       
        [HttpPost]
        public async Task<IActionResult> EditAdmin(EditAdminVM vm)
        {
            if (!ModelState.IsValid)
            {
                await LoadRolesAsync(vm);
                return View(vm);
            }

            var result = await _adminService.UpdateAdminAsync(vm.Admin, vm.Admin.Id);

            if (!result)
            {
                ModelState.AddModelError("", "Failed to update admin");
                await LoadRolesAsync(vm);
                return View(vm);
            }

            TempData["SuccessMessage"] = "Admin Updated Successfully";
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["ErrorMessage"] = $"Admin with {id} not found";
                return RedirectToAction(nameof(Index));
            }

            var result = await _adminService.DeleteAdminAsync(id);

            if (!result)
            {
                TempData["ErrorMessage"] = "Failed To Delete Admin";
            }

            TempData["SuccessMessage"] = "Admin Deleted Successfully";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Roles()
        {
            var roles = await _adminService.GetAllRolesAsync();
            return View(roles);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRoles()
        {
            var roles = await _adminService.GetAllRolesAsync();
            return View(roles);
        }

        [HttpPost]
        public async Task<ActionResult<string>> CreateRole(string roleName)
        {
            var result = await _adminService.CreateRoleAsync(roleName);
            if (!result)
                TempData["ErrorMessage"] = "Failed to create role , May be exist";
            else
                TempData["SuccessMessage"] = "Role Created Successfully";

            return RedirectToAction(nameof(Roles));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteRole(string roleName)
        {
            var result = await _adminService.DeleteRoleAsync(roleName);
            if (!result)
                TempData["ErrorMessage"] = "Failed to delete role";
            else
                TempData["SuccessMessage"] = "Role deleted successfully";

            return RedirectToAction(nameof(Roles));
        }

        [HttpPost]
        public async Task<IActionResult> AssignRoleToUser(string id, string roleName)
        {
            if (id == null)
            {
                TempData["ErrorMessage"] = $"Admin with {id} not found";
                return RedirectToAction(nameof(EditAdmin));
            }
            if (roleName == null)
            {
                TempData["ErrorMessage"] = $"Role name not found";
                return RedirectToAction(nameof(EditAdmin));
            }

            var result = await _adminService.AssignRoleToUserAsync(id, roleName);

            if (!result)
                TempData["ErrorMessage"] = $"can't assign role to user";
            else
                TempData["SuccessMessage"] = $"Role assigned successfully";

            return RedirectToAction(nameof(EditAdmin), new { id = id });
        }

        [HttpPost]
        public async Task<IActionResult> RemoveRoleFromUser(string id, string roleName)
        {

            if (id == null)
            {
                TempData["ErrorMessage"] = $"Admin with {id} not found";
                return RedirectToAction(nameof(EditAdmin));
            }
            if (roleName == null)
            {
                TempData["ErrorMessage"] = $"Role name not found";
                return RedirectToAction(nameof(EditAdmin));
            }

            var result = await _adminService.RemoveRoleFromUserAsync(id, roleName);

            if (!result)
                TempData["ErrorMessage"] = $"can't remove role to user";
            else
                TempData["SuccessMessage"] = $"Role removed successfully";

            return RedirectToAction(nameof(EditAdmin), new { id = id });
        }

        #region HelperMethod
        private async Task LoadRolesAsync(EditAdminVM vm)
        {
            vm.AllRoles = await _adminService.GetAllRolesAsync();
            vm.AssignedRoles = await _adminService.GetUserRolesAsync(vm.Admin.Id);
        }
        #endregion
    }
}
