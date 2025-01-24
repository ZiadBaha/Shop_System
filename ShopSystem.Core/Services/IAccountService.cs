using ShopSystem.Core.Dtos;
using ShopSystem.Core.Dtos.Account;
using ShopSystem.Core.Enums;
using ShopSystem.Core.Errors;
using ShopSystem.Core.Models;
using ShopSystem.Core.Models.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopSystem.Core.Services
{
    public interface IAccountService
    {
        Task<ContentContainer<object>> LoginAsync(Login dto);
        Task<ContentContainer<object>> LogoutAsync(string userId);

        Task<ApiResponse> ForgetPassword(string email);
        ApiResponse VerfiyOtp(VerifyOtp dto);
        Task SendEmailAsync(string To, string Subject, string Body, CancellationToken Cancellation = default);
        Task<bool> ConfirmUserEmailAsync(string userId, string token);
        Task<ApiResponse> ResetPasswordAsync(ResetPassword dto);


    }
}
