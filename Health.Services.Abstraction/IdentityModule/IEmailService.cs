using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Services.Abstraction.IdentityModule
{
    public interface IEmailService
    {
        Task<bool> SendOtpAsync(string toEmail, string userName, string otpCode);
    }
}
