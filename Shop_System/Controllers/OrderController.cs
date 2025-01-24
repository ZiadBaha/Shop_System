using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ShopSystem.Core.Dtos.Program.Ivoice;
using ShopSystem.Core.Dtos.Program;
using ShopSystem.Core.Dtos;
using ShopSystem.Core.Errors;
using ShopSystem.Core.Models.Identity;
using ShopSystem.Core.Models;
using ShopSystem.Core.Services.Programe;
using System.Security.Claims;

using AutoMapper;

namespace Shop_System.Controllers
{

    [Authorize]
    public class OrderController : ApiBaseController
    {
        private readonly IOrderRepository _orderService;
        private readonly ILogger<OrderController> _logger;
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;

        public OrderController(IOrderRepository orderService, ILogger<OrderController> logger,
            UserManager<AppUser> userManager, IMapper mapper)
        {
            _orderService = orderService;
            _logger = logger;
            _userManager = userManager;
            _mapper = mapper;
        }

        // Create a new order
        // POST: api/orders
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDTO createOrderDto)
        {
            // Validate the request data
            if (createOrderDto == null || !ModelState.IsValid)
            {
                return BadRequest(new ContentContainer<string>(null, "Invalid order data."));
            }

            try
            {
                var email = User.FindFirst(ClaimTypes.Email)?.Value;
                if (email == null)
                {
                    return BadRequest(new ApiResponse(400, "Invalid user"));
                }
                var user = await _userManager.FindByEmailAsync(email);

                if (string.IsNullOrEmpty(user.Id))
                {
                    return Unauthorized(new ContentContainer<string>(null, "User ID is missing."));
                }

                // Call the service to create the order
                var createdOrder = await _orderService.CreateOrderAsync(createOrderDto, user.Id);

                // Return a 201 Created response with the created order
                var response = new ContentContainer<OrderDTO>(createdOrder, "Order created successfully.");
                return CreatedAtAction(nameof(GetOrderById), new { id = createdOrder.Id }, response);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "An error occurred while creating the order.");
                return NotFound(new ContentContainer<string>(null, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred.");
                return StatusCode(500, new ContentContainer<string>(null, "An unexpected error occurred while processing the order."));
            }
        }




        // Get an order by ID
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            try
            {
                var order = await _orderService.GetOrderByIdAsync(id);
                if (order == null)
                {
                    return NotFound(new ContentContainer<string>(null, "Order not found"));
                }
                return Ok(new ContentContainer<GetOrderDTO>(order));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching the order.");
                return StatusCode(500, new ContentContainer<string>(null, "An error occurred while fetching the order."));
            }
        }



        // GET: api/orders/sumofamountofallorders
        [HttpGet("sumofamountofallorders")]
        public async Task<IActionResult> SumOfAmountOfAllOrders()
        {
            try
            {
                // Call the service method to get the total sum of all orders
                var totalAmount = await _orderService.SumOfAmountOfAllOrdersAsync();

                if (totalAmount == 0)
                    return NotFound(new ContentContainer<decimal>(0, "No orders found to calculate the total amount."));

                // Return the sum of all orders wrapped in ContentContainer
                return Ok(new ContentContainer<decimal>(totalAmount, "Total amount of all orders retrieved successfully."));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the total amount of all orders.");
                return StatusCode(500, new ContentContainer<string>(null, "An error occurred while processing your request."));
            }
        }



        // Get all orders with pagination and query options
        [HttpGet]
        public async Task<IActionResult> GetAllOrders([FromQuery] PaginationParameters paginationParameters, [FromQuery] QueryOptions queryOptions)
        {
            try
            {
                var orders = await _orderService.GetAllOrdersAsync(paginationParameters, queryOptions);
                return Ok(new ContentContainer<PagedResult<GetOrderDTO>>(orders));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching all orders.");
                return StatusCode(500, new ContentContainer<string>(null, "An error occurred while fetching all orders."));
            }
        }


        [HttpGet("customer/{customerId}/orders")]
        public async Task<IActionResult> GetCustomerOrdersAsync(int customerId, [FromQuery] PaginationParameters paginationParameters)
        {
            try
            {
                var result = await _orderService.GetCustomerOrdersAsync(customerId, paginationParameters);
                if (result == null || result.Items.Count() == 0)
                {
                    return Ok(new ContentContainer<PagedResult<GetOrderDTO>>(null, "No orders found for the specified customer."));
                }
                return Ok(new ContentContainer<PagedResult<GetOrderDTO>>(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching orders for Customer ID {customerId}: {ex.Message}");
                return StatusCode(500, new ContentContainer<string>(null, "An error occurred while fetching customer orders."));
            }
        }



        //[HttpPut("{id:int}")]
        //public async Task<IActionResult> UpdateOrder(int id, [FromBody] OrderDTO OrderDTO)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(new ContentContainer<string>(null, "Invalid input data"));
        //    }

        //    try
        //    {
        //        var updatedOrder = await _orderService.UpdateOrderAsync(id, OrderDTO);
        //        return Ok(new ContentContainer<OrderDTO>(updatedOrder));
        //    }
        //    catch (KeyNotFoundException ex)
        //    {
        //        _logger.LogWarning(ex, "Order not found for update.");
        //        return NotFound(new ContentContainer<string>(null, ex.Message));
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error occurred while updating the order.");
        //        return StatusCode(500, new ContentContainer<string>(null, "An error occurred while updating the order."));
        //    }
        //}


        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateOrder(int id, [FromBody] CreateOrderDTO updateOrderDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ContentContainer<string>(null, "Invalid input data"));
            }

            try
            {
                // Attempt to update the order
                var updatedOrder = await _orderService.UpdateOrderAsync(id, updateOrderDto, User.Identity.Name);

                // Return the updated order in a standardized response
                return Ok(new ContentContainer<OrderDTO>(updatedOrder, "Order updated successfully"));
            }
            catch (KeyNotFoundException ex)
            {
                // Log a warning and return 404 if the order ID was not found
                _logger.LogWarning(ex, "Order not found for update with ID: {OrderId}", id);
                return NotFound(new ContentContainer<string>(null, $"Order with ID {id} was not found."));
            }
            catch (InvalidOperationException ex)
            {
                // Handle business logic-related errors (e.g., invalid product quantities)
                _logger.LogWarning(ex, "Business logic validation failed during order update for ID: {OrderId}", id);
                return BadRequest(new ContentContainer<string>(null, ex.Message));
            }
            catch (Exception ex)
            {
                // Log unexpected errors and return a 500 response
                _logger.LogError(ex, "Unexpected error occurred while updating order with ID: {OrderId}", id);
                return StatusCode(500, new ContentContainer<string>(null, "An internal server error occurred. Please try again later."));
            }
        }


        // Delete multiple orders
        [HttpDelete]
        public async Task<IActionResult> DeleteMultipleOrders([FromBody] IEnumerable<int> ids)
        {
            if (ids == null || !ids.Any())
            {
                return BadRequest(new ContentContainer<string>(null, "Order IDs must be provided"));
            }

            try
            {
                var deletedCount = await _orderService.DeleteMultipleOrdersAsync(ids);
                return Ok(new ContentContainer<int>(deletedCount, $"{deletedCount} orders deleted successfully."));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting orders.");
                return StatusCode(500, new ContentContainer<string>(null, "An error occurred while deleting the orders."));
            }
        }

        // Calculate the total value of an order
        [HttpGet("{id:int}/calculate-total")]
        public async Task<IActionResult> CalculateTotalOrderValue(int id)
        {
            try
            {
                var totalValue = await _orderService.CalculateTotalOrderValueAsync(id);
                return Ok(new ContentContainer<decimal>(totalValue, "Total order value calculated successfully."));
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Order not found for total value calculation.");
                return NotFound(new ContentContainer<string>(null, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while calculating total order value.");
                return StatusCode(500, new ContentContainer<string>(null, "An error occurred while calculating total order value."));
            }
        }



        [HttpGet("{id:int}/invoice")]
        public async Task<IActionResult> GetOrderInvoice(int id)
        {
            try
            {
                var invoice = await _orderService.GenerateInvoiceAsync(id);
                return Ok(new ContentContainer<InvoiceDTO>(invoice, "Invoice generated successfully."));
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Order not found for invoice generation.");
                return NotFound(new ContentContainer<string>(null, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while generating the invoice.");
                return StatusCode(500, new ContentContainer<string>(null, "An error occurred while generating the invoice."));
            }
        }




        [HttpGet("generate/{orderId}")]
        public async Task<IActionResult> GenerateInvoice(int orderId)
        {
            try
            {
                var invoice = await _orderService.GenerateInvoiceAsync(orderId);

                if (invoice == null)
                {
                    return NotFound($"Order with ID {orderId} not found.");
                }

                var targetDirectory = @"D:\System"; 

                if (!Directory.Exists(targetDirectory))
                {
                    Directory.CreateDirectory(targetDirectory);
                }

                var filePath = Path.Combine(targetDirectory, $"Invoice_{orderId}.pdf");

                _logger.LogInformation($"Saving invoice to: {filePath}");

                _orderService.GenerateInvoicePdf(invoice, filePath);

                if (!System.IO.File.Exists(filePath))
                {
                    return StatusCode(500, "Failed to generate the invoice PDF.");
                }

                var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
                return File(fileBytes, "application/pdf", Path.GetFileName(filePath));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while generating the invoice for order ID {orderId}.");
                return StatusCode(500, $"An error occurred while generating the invoice: {ex.Message}");
            }
        }






        [HttpPost("print/{orderId}")]
        public async Task<IActionResult> PrintInvoice(int orderId, [FromQuery] string printerName)
        {
            try
            {
                if (string.IsNullOrEmpty(printerName))
                {
                    return BadRequest("Printer name is required.");
                }

                await _orderService.PrintInvoiceAsync(orderId, printerName);
                return Ok($"Order {orderId} has been sent to the printer {printerName}.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while printing the invoice for order ID {orderId}.");
                return StatusCode(500, $"An error occurred while printing the invoice: {ex.Message}");
            }
        }

      


    }
}
