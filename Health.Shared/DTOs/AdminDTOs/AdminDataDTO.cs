using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Shared.DTOs.AdminDTOs
{
    public class AdminDataDTO
    {
        public string Id { get; set; } = default!;
        public string FullName { get; set; } = default!;
        public string UserName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Phone { get; set; } = default!;
        public string CreatedAt { get; set; } = default!;
        public string Role { get; set; } = default!;
    }
}
