using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Health.Shared.DTOs.AdminDTOs
{
    public class CreateAdminDTO
    {
        [Required(ErrorMessage = "First name is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "First name must be between 2 and 50 characters")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "First name must contain letters only")]
        public string FirstName { get; set; } = default!;

        [Required(ErrorMessage = "Last name is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Last name must be between 2 and 50 characters")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Last name must contain letters only")]
        public string LastName { get; set; } = default!;

        [Required(ErrorMessage = "Phone number is required")]
        [RegularExpression(@"^\+?\d{10,15}$", ErrorMessage = "Invalid phone number format")]
        public string Phone { get; set; } = default!;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [StringLength(100)]
        public string Email { get; set; } = default!;

        [Required(ErrorMessage = "Password is required")]
        [StringLength(15, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 15 characters")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d@$!%*?&_]{8,15}$",
            ErrorMessage = "Password must contain at least one letter and one number")]
        public string Password { get; set; } = default!;

        [Required(ErrorMessage = "Role is required")]
        [RegularExpression("^(Admin|SuperAdmin)$", ErrorMessage = "Role must be Admin or SuperAdmin")]
        public string Role { get; set; } = default!;
    }
}
