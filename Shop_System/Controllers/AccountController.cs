using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ShopSystem.Core.Dtos;
using ShopSystem.Core.Dtos.Account;
using ShopSystem.Core.Enums;
using ShopSystem.Core.Models;
using ShopSystem.Core.Models.Account;
using ShopSystem.Core.Services;
using ShopSystem.Repository.Reposatories;
using ShopSystem.Service;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace Shop_System.Controllers
{
    public class AccountController : ApiBaseController
    {
        private readonly IAccountService _accountService;
        private readonly IFileService _fileService;
        private readonly ILogger<AdminController> _logger;


        public AccountController(IAccountService accountService, IFileService fileService, ILogger<AdminController> logger)
        {
            _accountService = accountService;
            _fileService = fileService;
            _logger = logger;

        }
        // test 


        [HttpPost("login")]
        public async Task<IActionResult> Login(Login dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _accountService.LoginAsync(dto);
            if (result.Message == "User not found." || result.Message == "Incorrect email or password.")
            {
                return BadRequest(result.Message);
            }
            return Ok(result);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromForm] string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("User ID is required.");
            }

            var result = await _accountService.LogoutAsync(userId);
            if (result.Message == "User not found.")
            {
                return BadRequest(result.Message);
            }

            return Ok(result);
        }



        [HttpPost("forgetPassword")]
        public async Task<IActionResult> ForgetPassword([FromHeader][EmailAddress] string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("Email address is required.");
            }
            try
            {
                var result = await _accountService.ForgetPassword(email);

                if (result.StatusCode == 200)
                {
                    return Ok("Password reset email sent successfully.");
                }
                else
                {
                    return StatusCode(result.StatusCode, result.Message);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An unexpected error occurred. Please try again later.");
            }
        }

        [HttpPost("verfiyOtp")]
        public IActionResult VerfiyOtp(VerifyOtp dto)
        {
            var result = _accountService.VerfiyOtp(dto);

            if (result.StatusCode == 200)
            {
                return Ok(result.Message);
            }
            else
            {
                return BadRequest(result.Message); 
            }
        }

        [HttpPut("resetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPassword dto)
        {
            var result = await _accountService.ResetPasswordAsync(dto);

            switch (result.StatusCode)
            {
                case 200:
                    return Ok(result.Message);
                case 400:
                    return BadRequest(result.Message);
                case 500:
                    return StatusCode(500, result.Message);
                default:
                    return StatusCode(500, "An unexpected error occurred.");
            }
        }
        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmUserEmail(string userId, string confirmationToken)
        {
            var result = await _accountService.ConfirmUserEmailAsync(userId!, confirmationToken!);

            if (result)
            {
                return RedirectPermanent(@"https://www.google.com/webhp?authuser=0");
            }
            else
            {
                return BadRequest("Failed to confirm user email.");
            }
        }

        private string GenerateCallBackUrl(string token, string userId)
        {
            var encodedToken = Uri.EscapeDataString(token);
            var encodedUserId = Uri.EscapeDataString(userId);
            var callBackUrl = $"{Request.Scheme}://{Request.Host}/api/Account/confirm-email?userId={encodedUserId}&confirmationToken={encodedToken}";
            return callBackUrl;
        }     
       
    }

}
