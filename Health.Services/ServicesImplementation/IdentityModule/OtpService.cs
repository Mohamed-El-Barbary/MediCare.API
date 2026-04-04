using AutoMapper;
using Health.Domain.Contracts;
using Health.Domain.Entities.IdentityModule;
using Health.Domain.Entities.IdentityModule.Enums;
using Health.Services.Abstraction.IdentityModule;
using Health.Services.Abstraction.IdentityModuleAbstraction;
using Health.Shared.CommonResponses;
using Health.Shared.DTOs.Enums;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using static System.Net.WebRequestMethods;

namespace Health.Services.ServicesImplementation.IdentityModule
{
    public class OtpService : IOtpService
    {
        private readonly int OtpLength = 6;
        private readonly int MaxAttempts = 5;
        private readonly TimeSpan OtpTtlMinutes = TimeSpan.FromMinutes(8);
        private readonly IOtpRepository _otpRepository;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;

        public OtpService(IOtpRepository otpRepository, IMapper mapper, IEmailService emailService)
        {
            _otpRepository = otpRepository;
            _mapper = mapper;
            _emailService = emailService;
        }

        public async Task<Result> SendOtpAsync(string email, OtpPurposeDTO purpose, string? ipAddress = null)
        {

            var otpCode = GenerateOtp();

            var otpEntry = new OtpEntry()
            {
                HashedCode = HashOtp(otpCode),
                IsVerified = false,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.Add(OtpTtlMinutes),
                AttemptCount = 0,
                Purpose = _mapper.Map<OtpPurpose>(purpose)
            };

            await _otpRepository.SetAsync(email, otpEntry, otpEntry.Purpose, OtpTtlMinutes);

            var emailSent = await _emailService.SendOtpAsync(email, "Mohame Elbarbray", otpCode);

            if (!emailSent)
                return Result.Fail(Error.Failure("Otp.EmailFailed", "Failed to send OTP email."));

            return Result.Ok();
        }

        public async Task<Result> VerifyOtpAsync(string email, string otp, OtpPurposeDTO purpose)
        {
            var otpEntry = await _otpRepository.GetAsync(email, (OtpPurpose)purpose);

            if (otpEntry == null)
                return Result.Fail(Error.Failure("Otp.NotFound", "OTP not found."));

            if (otpEntry.ExpiresAt < DateTime.UtcNow)
                return Result.Fail(Error.Failure("Otp.Expired", "OTP has expired."));

            if (otpEntry.AttemptCount >= MaxAttempts)
                return Result.Fail(Error.Failure("Otp.MaxAttemptsExceeded", "Too many failed attempts."));

            var remainingTtl = await _otpRepository.GetRemainingTtlAsync(email, otpEntry.Purpose);
            if (remainingTtl == null || remainingTtl <= TimeSpan.Zero)
                return Result.Fail(Error.Failure("Otp.Expired", "OTP has expired."));

            otpEntry.AttemptCount++;
            await _otpRepository.SetAsync(email, otpEntry, otpEntry.Purpose, OtpTtlMinutes);

            if (otpEntry.HashedCode != HashOtp(otp))
                return Result.Fail(Error.Failure("Otp.InvalidCode", "Invalid OTP code."));

            return Result.Ok();
        }

        public async Task<Result> MarkOtpVerifiedAsync(string email, OtpPurposeDTO purpose)
        {
            var otpEntry = await _otpRepository.GetAsync(email, (OtpPurpose)purpose);

            if (otpEntry == null)
                return Result.Fail(Error.Failure("Otp.NotFound", "OTP not found."));

            var remainingTtl = await _otpRepository.GetRemainingTtlAsync(email, otpEntry.Purpose);
            if (remainingTtl == null || remainingTtl <= TimeSpan.Zero)
                return Result.Fail(Error.Failure("Otp.Expired", "OTP has expired."));

            otpEntry.IsVerified = true;
            await _otpRepository.SetAsync(email, otpEntry, otpEntry.Purpose, remainingTtl.Value);

            return Result.Ok();
        }

        public async Task<Result<bool>> IsOtpVerifiedAsync(string email, OtpPurposeDTO purpose)
        {
            var otpEntry = await _otpRepository.GetAsync(email, (OtpPurpose)purpose);

            if (otpEntry == null)
                return Error.Failure("Otp.NotFound", "OTP not found.");

            return Result<bool>.Ok(otpEntry.IsVerified);
        }

        public async Task<Result> DeleteOtpAsync(string email, OtpPurposeDTO purpose)
        {
            await _otpRepository.DeleteAsync(email, (OtpPurpose)purpose);
            return Result.Ok();
        }

        #region Helper Methods

        private string GenerateOtp()
        {
            var number = RandomNumberGenerator.GetInt32(0, (int)Math.Pow(10, OtpLength));
            return number.ToString($"D{OtpLength}");
        }

        private string HashOtp(string otp)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(otp));
            return Convert.ToHexString(bytes);
        }

        #endregion

    }
}
