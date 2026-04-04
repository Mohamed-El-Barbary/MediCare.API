using Health.Shared.CommonResponses;
using Health.Shared.DTOs.Enums;
using System;
using System.Collections.Generic;
using System.Text;


namespace Health.Services.Abstraction.IdentityModuleAbstraction
{
    public interface IOtpService
    {
        Task<Result> SendOtpAsync(string email, OtpPurposeDTO purpose, string? ipAddress = null);
        Task<Result> VerifyOtpAsync(string email, string otp, OtpPurposeDTO purpose);
        Task<Result> MarkOtpVerifiedAsync(string email, OtpPurposeDTO purpose);
        Task<Result<bool>> IsOtpVerifiedAsync(string email, OtpPurposeDTO purpose);
        Task<Result> DeleteOtpAsync(string email, OtpPurposeDTO purpose);
        //Task<Result> GetOtpAsync(string email, OtpPurposeDTO purpose);

    }
}
