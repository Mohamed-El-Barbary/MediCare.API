using CloudinaryDotNet.Actions;
using Health.Services.Abstraction.IdentityModule;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace Health.Services.ServicesImplementation.IdentityModule
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<bool> SendOtpAsync(string toEmail, string userName, string otpCode)
        {
            var apiKey = _configuration["SendGrid:ApiKey"];
            var client = new SendGridClient(apiKey);

            var from = new EmailAddress(
                _configuration["SendGrid:FromEmail"],
                _configuration["SendGrid:FromName"]
            );
            var otpExpiryMinutes = int.Parse(_configuration["SendGrid:OtpExpiryMinutes"]!);

            var to = new EmailAddress(toEmail);
            var subject = "Your Verification Code";
            var htmlContent = BuildOtpEmailTemplate(userName, otpCode, otpExpiryMinutes);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, null, htmlContent);
            var response = await client.SendEmailAsync(msg);

            return response.IsSuccessStatusCode;
        }

        #region HelperMethod

        private static string BuildOtpEmailTemplate(string userName, string otpCode, int expiryMinutes)
        {
            return $$"""
                <!DOCTYPE html>
                <html lang="en">
                <head>
                    <meta charset="UTF-8"/>
                    <style>
                        body { margin:0; padding:0; background:#f4f6f8;
                                font-family:'Segoe UI', sans-serif; }
                
                        .container { max-width:520px; margin:40px auto;
                                      background:#fff; border-radius:12px;
                                      box-shadow:0 4px 20px rgba(0,0,0,0.08); }
                
                        .header { background:#1a73e8; padding:32px 40px; text-align:center; }
                        .header h1 { margin:0; color:#fff; font-size:22px; }
                
                        .body { padding:40px; color:#333; }
                
                        .otp-box { background:#f0f4ff; border:2px dashed #1a73e8;
                                    border-radius:10px; text-align:center;
                                    padding:20px; }
                
                        .otp-code { font-size:42px; font-weight:700;
                                     color:#1a73e8; letter-spacing:10px; }
                
                        .expiry { font-size:13px; color:#e53935; margin-top:10px; }
                
                        .warning { font-size:13px; color:#888; border-top:1px solid #eee;
                                    padding-top:24px; line-height:1.6; }
                
                        .footer { background:#f9f9f9; text-align:center;
                                   padding:20px; font-size:12px; color:#aaa; }
                    </style>
                </head>
                <body>
                    <div class="container">
                        <div class="header">
                            <h1>HealthCare — Verification Code</h1>
                        </div>
                
                        <div class="body">
                            <p>Hello, <strong>{{userName}}</strong> 👋</p>
                
                            <p>Use the code below to proceed. Do not share it with anyone.</p>
                
                            <div class="otp-box">
                                <div class="otp-code">{{otpCode}}</div>
                                <div class="expiry">⏱ Expires in {{expiryMinutes}} minutes</div>
                            </div>
                
                            <p class="warning">
                                If you did not request this, please ignore this email.
                                Your account remains secure.
                            </p>
                        </div>
                
                        <div class="footer">
                            © {{DateTime.UtcNow.Year}} HealthCare App. All rights reserved.
                        </div>
                    </div>
                </body>
                </html>
                """;
        }

        #endregion

    }
}
