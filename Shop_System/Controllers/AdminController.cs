using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShopSystem.Core.Dtos.Account;
using ShopSystem.Core.Dtos;
using ShopSystem.Core.Models;
using ShopSystem.Core.Services;
using ShopSystem.Core.Enums;
using ShopSystem.Core.Models.Account;
using ShopSystem.Repository.Reposatories;
using ShopSystem.Repository.Reposatories.Programe;
using ShopSystem.Core.Dtos.Program;

namespace Shop_System.Controllers
{

    public class AdminController : ApiBaseController
    {
        private readonly IAccountService _accountService;
        private readonly IAdminServices _adminServices;
        private readonly IFileService _fileService;
        private readonly ILogger<AdminController> _logger;


        public AdminController(IAccountService accountService, 
            IFileService fileService, IAdminServices adminServices
            , ILogger<AdminController> logger)
        {
            _accountService = accountService;
            _fileService = fileService;
            _adminServices = adminServices;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] Register model)
        {
            // Validate the model state
            if (!ModelState.IsValid)
            {
                return BadRequest(new ContentContainer<string>(null, "Please provide valid data"));
            }

            // Handle the role assignment
            string roleName;
            switch (model.Role)
            {
                case UserRole.User:
                    roleName = "User";
                    break;
                case UserRole.Admin:
                    roleName = "Admin";
                    break;
                default:
                    return BadRequest(new ContentContainer<string>(null, "Invalid role selected"));
            }

            // Handle image file upload
            if (model.ImageFile != null)
            {
                var fileResult = _fileService.SaveImage(model.ImageFile);
                if (fileResult.Item1 == 1)
                {
                    model.Image = fileResult.Item2; // getting name of image
                }
                else
                {
                    return BadRequest(new ContentContainer<string>(null, "Error uploading image"));
                }
            }

            try
            {
                // Pass the role name to the account service for registration
                var result = await _adminServices.RegisterAsync(model, roleName, GenerateCallBackUrl);

                if (result.StatusCode == 200)
                {
                    return Ok(new ContentContainer<string>(result.Message, "Registration successful"));
                }
                else
                {
                    return StatusCode(result.StatusCode, new ContentContainer<string>(null, result.Message));
                }
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ContentContainer<string>(null, "An unexpected error occurred"));
            }
        }


        private string GenerateCallBackUrl(string token, string userId)
        {
            var encodedToken = Uri.EscapeDataString(token);
            var encodedUserId = Uri.EscapeDataString(userId);
            var callBackUrl = $"{Request.Scheme}://{Request.Host}/api/Account/confirm-email?userId={encodedUserId}&confirmationToken={encodedToken}";
            return callBackUrl;
        }

      



        [HttpGet("all-users-session-data")]
        public async Task<IActionResult> GetAllUsersWithSessionData()
        {
            try
            {
                var response = await _adminServices.GetAllUsersWithSessionDataAsync();

                if (response.Data == null)
                {
                    return NotFound(new ContentContainer<object>(null, "No users found."));
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching users' session data.");

                return StatusCode(StatusCodes.Status500InternalServerError, new ContentContainer<object>(null, "An error occurred while processing your request."));
            }
        }

        [HttpGet("getUsers")]
        public async Task<IActionResult> GetUsers([FromQuery] PaginationParameters paginationParameters, [FromQuery] QueryOptions queryOptions)
        {
            try
            {
                var users = await _adminServices.GetUsersAsync(paginationParameters, queryOptions);
                return Ok(new ContentContainer<PagedResult<UserDto>>(users, "Users retrieved successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ContentContainer<string>(null, "An error occurred while retrieving users."));
            }
        }


        [HttpGet("getAllUsersSessionData")]
        public async Task<IActionResult> GetAllUsersSessionData()
        {
            var result = await _adminServices.GetAllUsersSessionDataAsync();

            if (result.Data == null || !result.Data.Any())
            {
                return NotFound(result.Message);
            }

            return Ok(result);
        }


        [HttpGet("usersCount")]
        public async Task<IActionResult> GetUsersCount()
        {
            try
            {
                var count = await _adminServices.GetUsersCountAsync();

                return Ok(new ContentContainer<int>(count, "User count retrieved successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ContentContainer<string>(null, $"An error occurred: {ex.Message}"));
            }
        }

        [HttpDelete("deleteUsers")]
        public async Task<IActionResult> DeleteUsers([FromForm] IEnumerable<string> userIds)
        {
            try
            {
                if (userIds == null || !userIds.Any())
                {
                    return BadRequest(new ContentContainer<string>(null, "No user IDs provided."));
                }

                var result = await _adminServices.DeleteMultipleUsersAsync(userIds);

                if (result.StatusCode == 200)
                {
                    return Ok(new ContentContainer<string>(result.Message, $"{result.Message}"));
                }
                else
                {
                    return StatusCode(result.StatusCode, new ContentContainer<string>(null, result.Message));
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ContentContainer<string>(null, $"An error occurred: {ex.Message}"));
            }
        }

        [HttpPut("UpdateUserInfo")]
        public async Task<IActionResult> UpdateUserInfo([FromForm] UpdateUserDto updateUserDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ContentContainer<string>(null, "Invalid input data."));
            }

            try
            {
                var result = await _adminServices.UpdateUserInfoAsync(updateUserDto);

                if (result.StatusCode == 200)
                {
                    return Ok(new ContentContainer<string>(result.Message, "User information updated successfully."));
                }

                return StatusCode(result.StatusCode, new ContentContainer<string>(null, result.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ContentContainer<string>(null, $"An error occurred: {ex.Message}"));
            }
        }



        [HttpGet("getUserInfo/{userId}")]
        public async Task<IActionResult> GetUserInfoById(string userId)
        {
            var result = await _adminServices.GetUserInfoByIdAsync(userId);
            if (result == null)
            {
                return NotFound(new ContentContainer<string>(null, "User not found."));
            }

            return Ok(new ContentContainer<UserDto>(result, "User information retrieved successfully."));
        }

        [HttpGet("profit/{id:int}")]
        public async Task<IActionResult> GetOrderProfit(int id)
        {
            try
            {
                var profit = await _adminServices.GetOrderProfitAsync(id);

                if (profit == null)
                {
                    return NotFound(new ContentContainer<string>(null, "Order not found"));
                }

                return Ok(new ContentContainer<decimal?>(profit, "Profit calculated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while calculating the profit for order ID: {OrderId}", id);
                return StatusCode(500, new ContentContainer<string>(null, "An error occurred while calculating the profit."));
            }
        }

        [HttpGet("profits")]
        public async Task<IActionResult> GetAllOrdersProfits([FromQuery] PaginationParameters paginationParameters, [FromQuery] QueryOptions queryOptions)
        {
            try
            {
                var result = await _adminServices.GetOrdersWithProfitAsync(paginationParameters, queryOptions);

                if (result == null || !result.Items.Any())
                {
                    return NotFound(new ContentContainer<string>(null, "No orders found"));
                }

                return Ok(new ContentContainer<PagedResult<OrderProfitDto>>(result, "Order profits retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching order profits.");
                return StatusCode(500, new ContentContainer<string>(null, "An error occurred while fetching order profits."));
            }
        }


        [HttpGet("todayOrders")]
        public async Task<IActionResult> GetTodayOrders()
        {
            try
            {
                var orders = await _adminServices.GetTodayOrdersAsync();

                var response = new ContentContainer<List<GetOrderDTO>>(orders, "Orders fetched successfully.");

                return Ok(response); 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching today's orders.");

                var errorResponse = new ContentContainer<string>(null, "An error occurred while processing your request.");

                return StatusCode(StatusCodes.Status500InternalServerError, errorResponse); 
            }
        }


    }
}
