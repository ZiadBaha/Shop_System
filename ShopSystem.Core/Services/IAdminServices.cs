using ShopSystem.Core.Dtos.Account;
using ShopSystem.Core.Dtos;
using ShopSystem.Core.Errors;
using ShopSystem.Core.Models.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShopSystem.Core.Models;
using ShopSystem.Core.Dtos.Program;

namespace ShopSystem.Core.Services
{
    public interface IAdminServices
    {

        Task<ApiResponse> RegisterAsync(Register dto, string roleName, Func<string, string, string> generateCallBackUrl);
        Task<UserDto> GetUserInfoByIdAsync(string userId);
        Task<ApiResponse> UpdateUserInfoAsync(UpdateUserDto updateUserDto);
        Task<PagedResult<UserDto>> GetUsersAsync(PaginationParameters paginationParameters, QueryOptions queryOptions);
        Task<ContentContainer<List<UserSessionDto>>> GetAllUsersSessionDataAsync();
        Task<int> GetUsersCountAsync();
        Task<ApiResponse> DeleteMultipleUsersAsync(IEnumerable<string> userIds);
        Task<decimal?> GetOrderProfitAsync(int orderId);
        Task<PagedResult<OrderProfitDto>> GetOrdersWithProfitAsync(PaginationParameters paginationParameters, QueryOptions queryOptions);
        Task<List<GetOrderDTO>> GetTodayOrdersAsync();
        Task<ContentContainer<object>> GetAllUsersWithSessionDataAsync();
    }
}
