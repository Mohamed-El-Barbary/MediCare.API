using Health.Domain.Contracts;
using Health.Domain.Entities.DoctorModule;
using Health.Domain.Entities.IdentityModule;
using Health.Services.Abstraction;
using Health.Shared.CommonResponses;
using Health.Shared.DTOs.IdentityDTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using Gender = Health.Domain.Entities.DoctorModule.Gender;

namespace Health.Services.ServicesImplementation
{
    public class AuthenticationService : IAuthenticationService
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;

        public AuthenticationService(
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration,
            IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _configuration = configuration;
            _unitOfWork = unitOfWork;
        }


        public async Task<Result<UserDTO>> RegisterAsync(RegisterDoctorDTO registerDTO)
        {
            if (await _userManager.FindByEmailAsync(registerDTO.Email) is not null)
                return Error.Validation("Email.Exists", "Email already exists.");

            if (!string.IsNullOrWhiteSpace(registerDTO.PhoneNumber))
            {
                var phoneExists = await _userManager.Users
                    .AnyAsync(u => u.PhoneNumber == registerDTO.PhoneNumber);

                if (phoneExists)
                    return Error.Validation("Phone.Exists", "Phone number already exists.");
            }


            var user = new ApplicationUser
            {
                FirstName = registerDTO.FirstName,
                LastName = registerDTO.LastName,
                Email = registerDTO.Email,
                UserName = registerDTO.Email,
                PhoneNumber = registerDTO.PhoneNumber
            };


            var identityResult = await _userManager.CreateAsync(user, registerDTO.Password);

            if (!identityResult.Succeeded)
            {
                var errors = identityResult.Errors
                    .Select(e => Error.Validation(e.Code, e.Description))
                    .ToList();

                return errors;
            }


            // 4️⃣ Assign Doctor Role
            const string role = "Doctor";
            await _userManager.AddToRoleAsync(user, role);

            // 5️⃣ Create DoctorProfile
            var doctorProfile = new DoctorProfile
            {
                UserId = user.Id,
                Gender = Gender.Male,
                Specialization = registerDTO.Specialization,
                YearsOfExperience = registerDTO.YearsOfExperience,
                Bio = registerDTO.Bio,
                ClinicLocation = registerDTO.ClinicLocation,
                SyndicateCardUrl = registerDTO.SyndicateCardUrl,
                DoctorPictureUrl = registerDTO.DoctorPictureUrl,
                Address = new Address
                {
                    City  = registerDTO.Address.City,
                    Country = registerDTO.Address.Country,
                    Street = registerDTO.Address.Street
                },
                JoinDate = DateTime.UtcNow
            };

            await _unitOfWork.GetRepository<DoctorProfile, int>().AddAsync(doctorProfile);
            await _unitOfWork.SaveChanges();

            return new UserDTO(
                user.Id,
                user.Email!,
                user.FirstName,
                role,
                "accessToken",
                DateTime.Now
            );
        }


    }
}
