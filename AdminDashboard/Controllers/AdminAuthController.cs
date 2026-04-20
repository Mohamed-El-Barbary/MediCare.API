using AdminDashboard.Models;
using Health.Domain.Entities.IdentityModule;
using Health.Services.Abstraction.IdentityModuleAbstraction;
using Health.Shared.DTOs.IdentityDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AdminDashboard.Controllers
{
    public class AdminAuthController : Controller
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminAuthController(IAuthenticationService authenticationService,
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager)
        {
            _authenticationService = authenticationService;
            _signInManager = signInManager;
            _userManager = userManager;

        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> LoginAdmin(LoginVM loginVM)
        {
            if (!ModelState.IsValid)
                return View("Index", loginVM);


            var user = await _userManager.FindByEmailAsync(loginVM.Email);

            if (user == null)
            {
                ModelState.AddModelError("", "User not found");
                return View("Index", loginVM);
            }

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, loginVM.Password);

            if (!isPasswordValid)
            {
                ModelState.AddModelError("", "Invalid password");
                return View("Index", loginVM);
            }

            await _signInManager.SignInAsync(user, isPersistent: true);

            TempData["SuccessMessage"] = "Login successful ";
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> LogoutAdmin()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "AdminAuth");
        }
    }
}
