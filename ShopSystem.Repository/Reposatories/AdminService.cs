using iText.Commons.Actions.Contexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ShopSystem.Core.Dtos;
using ShopSystem.Core.Dtos.Account;
using ShopSystem.Core.Dtos.Program;
using ShopSystem.Core.Errors;
using ShopSystem.Core.Models;
using ShopSystem.Core.Models.Account;
using ShopSystem.Core.Models.Entites;
using ShopSystem.Core.Models.Identity;
using ShopSystem.Core.Services;
using ShopSystem.Repository.Data;
using ShopSystem.Repository.Data.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopSystem.Repository.Reposatories
{
    public class AdminService : IAdminServices
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly MailSettings _mailSettings;
        private readonly ITokenService _TokenService;
        private readonly IOtpService _otpService;
        private readonly IMemoryCache _cache;
        private readonly ILogger<AdminService> _logger;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly StoreContext _context;
        private readonly AppIdentityDbContext  _appIdentityDbContext;



        public AdminService(UserManager<AppUser> userManager,
            IOptionsMonitor<MailSettings> options,
            ITokenService tokenService,
            IOtpService otpService,
            IMemoryCache cache,
            ILogger<AdminService> logger,
            RoleManager<IdentityRole> roleManager,
            StoreContext Context,
            AppIdentityDbContext appIdentityDbContext
            )
        {
            _userManager = userManager;
            _mailSettings = options.CurrentValue;
            _TokenService = tokenService;
            _otpService = otpService;
            _cache = cache;
            _logger = logger;
            _roleManager = roleManager;
            _context = Context;
            _appIdentityDbContext = appIdentityDbContext;
        }


      

        public async Task<ApiResponse> RegisterAsync(Register dto, string roleName, Func<string, string, string> generateCallBackUrl)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user != null)
            {
                return new ApiResponse(400, "User with this email already exists.");
            }

            user = new AppUser
            {
                UserRole = dto.Role,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Image = dto.Image,
                ImageFile = dto.ImageFile,
                PhoneNumber = dto.PhoneNumber,
                UserName = dto.Email.Split('@')[0],
                EmailConfirmed = true,
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
            {
                return new ApiResponse(400, "Something went wrong with the data you entered");
            }

            await _userManager.AddToRoleAsync(user, roleName);

            var emailConfirmation = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var callBackUrl = generateCallBackUrl(emailConfirmation, user.Id);
            var emailBody = $"<h1>Dear {user.UserName}! Welcome To Shop_System.</h1><p>Please <a href='{callBackUrl}'>Click Here</a> To Confirm Your Email.</p>";

            return new ApiResponse(200, "User registered successfully.");
        }


      
        public async Task<ApiResponse> UpdateUserInfoAsync(UpdateUserDto updateUserDto)
        {
            var user = await _userManager.FindByIdAsync(updateUserDto.Id);
            if (user == null)
            {
                return new ApiResponse(404, "User not found.");
            }

            user.FirstName = updateUserDto.FirstName;
            user.LastName = updateUserDto.LastName;
            user.UserRole = updateUserDto.Role;
            user.PhoneNumber = updateUserDto.PhoneNumber;
            user.Email = updateUserDto.Email;

            if (!string.IsNullOrEmpty(updateUserDto.Password) && !string.IsNullOrEmpty(updateUserDto.ConfirmPassword))
            {
                if (updateUserDto.Password != updateUserDto.ConfirmPassword)
                {
                    return new ApiResponse(400, "Passwords do not match.");
                }

                var passwordChangeResult = await _userManager.RemovePasswordAsync(user);
                if (!passwordChangeResult.Succeeded)
                {
                    return new ApiResponse(400, "Failed to remove old password.");
                }

                passwordChangeResult = await _userManager.AddPasswordAsync(user, updateUserDto.Password);
                if (!passwordChangeResult.Succeeded)
                {
                    return new ApiResponse(400, "Failed to set new password.");
                }
            }

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return new ApiResponse(400, "Failed to update user information.");
            }

            return new ApiResponse(200, "User information updated successfully.");
        }


        public async Task<ContentContainer<List<UserSessionDto>>> GetAllUsersSessionDataAsync()
        {
            var users = await _userManager.Users.ToListAsync();

            if (users == null || !users.Any())
            {
                return new ContentContainer<List<UserSessionDto>>(null, "No users found.");
            }

            var userSessions = users.Select(user => new UserSessionDto
            {
                UserId = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                LoginTime = user.LoginTime,
                LogoutTime = user.LogoutTime,
                SessionDuration = (user.LogoutTime.HasValue && user.LoginTime.HasValue)
                    ? user.LogoutTime.Value - user.LoginTime.Value
                    : TimeSpan.Zero
            }).ToList();

            return new ContentContainer<List<UserSessionDto>>(userSessions);
        }

      


        public async Task<ContentContainer<object>> GetAllUsersWithSessionDataAsync()
        {
            var users = await _userManager.Users.ToListAsync();

            if (users == null || !users.Any())
            {
                return new ContentContainer<object>(null, "No users found.");
            }

            var usersData = new List<object>();

            foreach (var user in users)
            {
                if (user.LoginTime != null && user.LogoutTime != null)
                {
                    var sessionDuration = user.LogoutTime.Value - user.LoginTime.Value;

                    var userSessionData = new
                    {
                        UserId = user.Id,
                        loginTime = user.LoginTime,
                        logoutTime = user.LogoutTime,
                        username = user.UserName,
                        SessionDuration = sessionDuration.ToString(@"hh\:mm\:ss")
                    };

                    usersData.Add(userSessionData);
                }
                else
                {
                    var userSessionData = new
                    {
                        UserId = user.Id,
                        loginTime = user.LoginTime,
                        logoutTime = user.LogoutTime,
                        username = user.UserName,
                        SessionDuration = "N/A"

                    };

                    usersData.Add(userSessionData);
                }
            }

            return new ContentContainer<object>(usersData, "Users and session data fetched successfully.");
        }




        public async Task<PagedResult<UserDto>> GetUsersAsync(PaginationParameters paginationParameters, QueryOptions queryOptions)
        {
            var query = _userManager.Users.AsQueryable();

            if (!string.IsNullOrEmpty(queryOptions.Search))
            {
                query = query.Where(u => u.FirstName.Contains(queryOptions.Search) ||
                                         u.LastName.Contains(queryOptions.Search) ||
                                         u.Email.Contains(queryOptions.Search));
            }

            if (!string.IsNullOrEmpty(queryOptions.Name))
            {
                query = query.Where(u => u.UserRole.ToString().Equals(queryOptions.Name, StringComparison.OrdinalIgnoreCase));
            }
            

            query = queryOptions.SortField.ToLower() switch
            {
                "id" => queryOptions.SortDescending ? query.OrderByDescending(u => u.Id) : query.OrderBy(u => u.Id),
                "firstname" => queryOptions.SortDescending ? query.OrderByDescending(u => u.FirstName) : query.OrderBy(u => u.FirstName),
                "lastname" => queryOptions.SortDescending ? query.OrderByDescending(u => u.LastName) : query.OrderBy(u => u.LastName),
                "email" => queryOptions.SortDescending ? query.OrderByDescending(u => u.Email) : query.OrderBy(u => u.Email),
                _ => queryOptions.SortDescending ? query.OrderByDescending(u => u.FirstName) : query.OrderBy(u => u.FirstName), // Default sorting by FirstName
            };

            var totalCount = await query.CountAsync();

            var users = await query
                .Skip((paginationParameters.PageNumber - 1) * paginationParameters.PageSize)
                .Take(paginationParameters.PageSize)
                .ToListAsync();

            var userDtos = users.Select(user => new UserDto
            {
                id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Role = user.UserRole,

            }).ToList();

            return new PagedResult<UserDto>
            {
                Items = userDtos,
                TotalCount = totalCount,
                PageNumber = paginationParameters.PageNumber,
                PageSize = paginationParameters.PageSize
            };
        }

   
        public async Task<int> GetUsersCountAsync()
        {
            var usersCount = await _userManager.Users.CountAsync();
            return usersCount;
        }

        public async Task<ApiResponse> DeleteMultipleUsersAsync(IEnumerable<string> userIds)
        {
            var usersToDelete = new List<AppUser>();

            foreach (var userId in userIds)
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    usersToDelete.Add(user);
                }
            }

            if (!usersToDelete.Any())
            {
                return new ApiResponse(404, "No users found for deletion.");
            }

            int deletedCount = 0;

            // Delete users one by one
            foreach (var user in usersToDelete)
            {
                var result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    deletedCount++;
                }
                else
                {
                    return new ApiResponse(400, "Failed to delete some or all users.");
                }
            }

            return new ApiResponse(200, $"{deletedCount} user(s) deleted successfully.");
        }
        public async Task<UserDto> GetUserInfoByIdAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return null;
            }

            return new UserDto
            {

                id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email,
                Role = user.UserRole,

            };
        }



        public async Task<decimal?> GetOrderProfitAsync(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                return null;
            }

            decimal totalProfit = order.OrderItems.Sum(oi =>
            {
                decimal sellingPrice = oi.Product?.SellingPrice ?? 0;
                decimal purchasePrice = oi.Product?.PurchasePrice ?? 0;
                return (sellingPrice - purchasePrice) * oi.Quantity;
            });

            return totalProfit;
        }



        public async Task<PagedResult<OrderProfitDto>> GetOrdersWithProfitAsync(PaginationParameters paginationParameters, QueryOptions queryOptions)
        {
            try
            {
                var query = _context.Orders
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product)
                    .AsQueryable();

                if (queryOptions.StartDate.HasValue)
                {
                    query = query.Where(o => o.OrderDate >= queryOptions.StartDate.Value);
                }
                if (queryOptions.EndDate.HasValue)
                {
                    query = query.Where(o => o.OrderDate <= queryOptions.EndDate.Value);
                }

                var queryWithProfit = query.Select(o => new OrderProfitDto
                {
                    CashierId=o.UserId,
                    OrderId = o.Id,
                    OrderDate = o.OrderDate,
                    Profit = o.OrderItems.Sum(oi => (oi.Product.SellingPrice - oi.Product.PurchasePrice) * oi.Quantity)
                });

                if (!string.IsNullOrEmpty(queryOptions.SortField))
                {
                    if (queryOptions.SortField.Equals("Profit", StringComparison.OrdinalIgnoreCase))
                    {
                        queryWithProfit = queryOptions.SortDescending
                            ? queryWithProfit.OrderByDescending(o => o.Profit)
                            : queryWithProfit.OrderBy(o => o.Profit);
                    }
                    else if (queryOptions.SortField.Equals("OrderDate", StringComparison.OrdinalIgnoreCase))
                    {
                        queryWithProfit = queryOptions.SortDescending
                            ? queryWithProfit.OrderByDescending(o => o.OrderDate)
                            : queryWithProfit.OrderBy(o => o.OrderDate);
                    }
                }

                var totalItems = await queryWithProfit.CountAsync();

                var paginatedResult = await queryWithProfit
                    .Skip((paginationParameters.PageNumber - 1) * paginationParameters.PageSize)
                    .Take(paginationParameters.PageSize)
                    .ToListAsync();

                return new PagedResult<OrderProfitDto>
                {
                    Items = paginatedResult,
                    TotalCount = totalItems,
                    PageNumber = paginationParameters.PageNumber,
                    PageSize = paginationParameters.PageSize
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching orders with profit.");
                throw;
            }
        }


        public async Task<List<GetOrderDTO>> GetTodayOrdersAsync()
        {
            try
            {
                var today = DateTime.Today;

                var orders = await _context.Orders
                    .Include(o => o.Customer)
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product)
                    .Where(o => o.OrderDate.Date == today) 
                    .ToListAsync();

                var userIds = orders.Select(o => o.UserId).Distinct().ToList();
                var users = await _appIdentityDbContext.Users
                    .Where(u => userIds.Contains(u.Id))
                    .ToDictionaryAsync(u => u.Id, u => u);

                var orderDtos = orders.Select(order => new GetOrderDTO
                {
                    Id = order.Id,
                    OrderDate = order.OrderDate,
                    Notes = order.Notes,
                    TotalAmount = order.TotalAmount,
                    TotalDiscount = order.TotalDiscount,
                    FinalAmount = order.TotalAmount - order.TotalDiscount,
                    Customer = new GetCustomerDTO
                    {
                        Id = order.Customer.Id,
                        Name = order.Customer.Name ?? "Unknown Customer",
                        Phone = order.Customer.Phone ?? "Unknown Phone"
                    },
                    UserId = order.UserId,
                    User = users.ContainsKey(order.UserId) ? new UserDto
                    {
                        id = users[order.UserId].Id,
                        FirstName = users[order.UserId].FirstName ?? "Unknown First Name",
                        LastName = users[order.UserId].LastName ?? "Unknown Last Name",
                        Email = users[order.UserId].Email ?? "Unknown Email",
                        PhoneNumber = users[order.UserId].PhoneNumber ?? "Unknown Phone",
                    } : null,
                    OrderItems = order.OrderItems.Select(oi => new GetOrderItemDTO
                    {
                        ProductId = oi.Product?.Id ?? 0,
                        ProductName = oi.Product?.Name ?? "Unknown Product",
                        SellingPrice = oi.Product?.SellingPrice ?? 0,
                        Quantity = oi.Quantity,
                        Discount = oi.Discount,
                        Subtotal = oi.Quantity * (oi.Product?.SellingPrice ?? 0) * (1 - oi.Discount / 100)
                    }).ToList()
                }).ToList();

                return orderDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching today's orders.");
                throw;
            }
        }

    }
}
