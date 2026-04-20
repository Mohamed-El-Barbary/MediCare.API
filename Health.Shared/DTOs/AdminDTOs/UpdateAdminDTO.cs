using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Health.Shared.DTOs.AdminDTOs
{
    public class UpdateAdminDTO
    {
        public string Id { get; set; } = default!;
        [StringLength(50, MinimumLength = 2, ErrorMessage = "First name must be between 2 and 50 characters")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "First name must contain letters only")]
        public string? FirstName { get; set; }

        [StringLength(50, MinimumLength = 2, ErrorMessage = "Last name must be between 2 and 50 characters")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Last name must contain letters only")]
        public string? LastName { get; set; } 

        [RegularExpression(@"^\+?\d{10,15}$", ErrorMessage = "Invalid phone number format")]
        public string? Phone { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email format")]
        [StringLength(100)]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Role is required")]
        [RegularExpression("^(Admin|SuperAdmin)$", ErrorMessage = "Role must be Admin or SuperAdmin")]
        public string Role { get; set; } = default!;
    }
}
