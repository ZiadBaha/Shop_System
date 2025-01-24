using MailKit.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using Org.BouncyCastle.Asn1.Ocsp;
using ShopSystem.Core.Dtos;
using ShopSystem.Core.Dtos.Account;
using ShopSystem.Core.Enums;
using ShopSystem.Core.Errors;
using ShopSystem.Core.Models;
using ShopSystem.Core.Models.Account;
using ShopSystem.Core.Models.Identity;
using ShopSystem.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopSystem.Repository.Reposatories
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly MailSettings _mailSettings;
        private readonly ITokenService _TokenService;
        private readonly IOtpService _otpService;
        private readonly IMemoryCache _cache;
        private readonly ILogger<AccountService> _logger;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountService(UserManager<AppUser> userManager,
            IOptionsMonitor<MailSettings> options,
            ITokenService tokenService,
            IOtpService otpService,
            IMemoryCache cache,
            ILogger<AccountService> logger,
            RoleManager<IdentityRole> roleManager
            )
        {
            _userManager = userManager;
            _mailSettings = options.CurrentValue;
            _TokenService = tokenService;
            _otpService = otpService;
            _cache = cache;
            _logger = logger;
            _roleManager = roleManager;
        }

        public async Task<ContentContainer<object>> LoginAsync(Login dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
            {
                return new ContentContainer<object>(null, "User not found.");
            }

            if (!await _userManager.CheckPasswordAsync(user, dto.Password))
            {
                return new ContentContainer<object>(null, "Incorrect email or password.");
            }

            if (!user.EmailConfirmed)
            {
                return new ContentContainer<object>(null, "Email not confirmed.");
            }

            user.LoginTime = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            var roles = await _userManager.GetRolesAsync(user);
            var token = await _TokenService.CreateTokenAsync(user);

            var responseData = new
            {
                Role = user.UserRole,
                firstName = user.FirstName,
                lastName = user.LastName,
                email = user.Email,
                UserId = user.Id,
                Token = token,
                loginTime = user.LoginTime
            };

            return new ContentContainer<object>(responseData, "Login successful.");
        }

        public async Task<ContentContainer<object>> LogoutAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return new ContentContainer<object>(null, "User not found.");
            }

            if (user.LoginTime == null)
            {
                return new ContentContainer<object>(null, "Login time not recorded. Cannot calculate duration.");
            }

            user.LogoutTime = DateTime.UtcNow;
            var sessionDuration = user.LogoutTime.Value - user.LoginTime.Value;
            await _userManager.UpdateAsync(user);

            var responseData = new
            {
                UserId = user.Id,
                logintime = user.LogoutTime,
                logoutTime = user.LogoutTime,
                SessionDuration = sessionDuration.ToString(@"hh\:mm\:ss")
            };

            return new ContentContainer<object>(responseData, "Logout successful.");
        }




        public async Task<ApiResponse> ForgetPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentException("Email address is required.", nameof(email));
            }
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return new ApiResponse(400, "User not found.");
            }

            var otp = _otpService.GenerateOtp(email);

            try
            {
                await SendEmailAsync(email,
                    "Verification Code",
                    $"Dear {user.UserName},<br/> Use this code to reset your password: <strong>{otp}</strong>. Keep it safe and do not share it with anyone.");

                return new ApiResponse(200, "Password reset email sent successfully.");
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, "An unexpected error occurred while sending the password reset email. Please try again later.");
            }
        }
        public ApiResponse VerfiyOtp(VerifyOtp dto)
        {
            var isValidOtp = _otpService.IsValidOtp(dto.Email, dto.Otp);

            if (!isValidOtp)
            {
                return new ApiResponse(400, "Invalid OTP.");
            }
            return new ApiResponse(200, "Valid");
        }
        public async Task<ApiResponse> ResetPasswordAsync(ResetPassword dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user == null)
            {
                return new ApiResponse(400, "User not found.");
            }

            if (!_cache.TryGetValue(dto.Email, out bool validOtp) || !validOtp)
            {
                return new ApiResponse(400, "You have not verified your email addres(OTP).");
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var result = await _userManager.ResetPasswordAsync(user, token, dto.Password);

            if (result.Succeeded)
            {
                return new ApiResponse(200, "Password reset successfully.");
            }
            else
            {
                return new ApiResponse(500, "Failed to reset password.");
            }
        }
        public async Task<bool> ConfirmUserEmailAsync(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return false;
            }

            var confirmed = await _userManager.ConfirmEmailAsync(user, token);

            return confirmed.Succeeded;
        }
        public async Task SendEmailAsync(string To, string Subject, string Body, CancellationToken Cancellation = default)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_mailSettings.DisplayedName, _mailSettings.Email));
            message.To.Add(new MailboxAddress("", To));
            message.Subject = Subject;

            message.Body = new TextPart("html")
            {
                Text = Body
            };

            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                await client.ConnectAsync(_mailSettings.SmtpServer, _mailSettings.Port,
                    SecureSocketOptions.StartTls, Cancellation);
                await client.AuthenticateAsync(_mailSettings.Email, _mailSettings.Password, Cancellation);
                await client.SendAsync(message, Cancellation);
                await client.DisconnectAsync(true, Cancellation);
            }
        }


      



    }

}
