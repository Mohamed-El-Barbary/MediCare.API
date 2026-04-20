using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Shared.DTOs.AdminDTOs
{
    public class ChangeAdminPasswordDTO
    {
        public string CurrentPassword { get; set; } = default!;
        public string NewPassword { get; set; } = default!;

    }
}
