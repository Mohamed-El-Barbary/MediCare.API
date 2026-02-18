using Health.Domain.Contracts;
using Health.Domain.Entities.IdentityModule;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Persistence.IdentityData.DataSeed
{
    public class IdentityDataInitializer : IDataInitializer
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<IdentityDataInitializer> _logger;

        public IdentityDataInitializer(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<IdentityDataInitializer> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        public async Task InitializeAsync()
        {

            try
            {

                if (!_roleManager.Roles.Any())
                {
                    await _roleManager.CreateAsync(new IdentityRole("SuperAdmin"));
                    await _roleManager.CreateAsync(new IdentityRole("Admin"));
                    await _roleManager.CreateAsync(new IdentityRole("Doctor"));
                    await _roleManager.CreateAsync(new IdentityRole("Patient"));
                }

                if (!_userManager.Users.Any())
                {
                    var user01 = new ApplicationUser()
                    {
                        FirstName = "Mohamed",
                        LastName = "Elbarbary",
                        UserName = "MohamedElbarbary",
                        Email = "mohamedelbarbary511@gmail.com",
                        PhoneNumber = "01092814027"
                    };
                    var user02 = new ApplicationUser()
                    {
                        FirstName = "Mohamed",
                        LastName = "Ali",
                        UserName = "MohamedAli",
                        Email = "MohamedAli511@gmail.com",
                        PhoneNumber = "01212859631"
                    };

                    await _userManager.CreateAsync(user01, "P@ssw0rd");
                    await _userManager.CreateAsync(user02, "P@ssw0rd");

                    await _userManager.AddToRoleAsync(user01, "SuperAdmin");
                    await _userManager.AddToRoleAsync(user02, "Admin");
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error While Seeding Identity Database : Message = {ex.Message} ");
            }

        }
    }
}
