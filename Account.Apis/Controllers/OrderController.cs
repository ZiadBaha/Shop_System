using Account.Core.Dtos.Program;
using Account.Core.Dtos;
using Account.Core.Models;
using Account.Reposatory.Reposatories.Programe;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Account.Core.Services.Programe;

namespace Account.Apis.Controllers
{

    public class OrderController : ApiBaseController
    {
        private readonly IOrderRepository _orderService;
        private readonly ILogger<OrderController> _logger;

        public OrderController(IOrderRepository orderService, ILogger<OrderController> logger)
        {
            _orderService = orderService;
            _logger = logger;
        }

        // GET: api/orders
        [HttpGet]
        public async Task<IActionResult> GetAllOrders([FromQuery] PaginationParameters paginationParameters, [FromQuery] QueryOptions queryOptions)
        {
            var result = await _orderService.GetAllOrdersAsync(paginationParameters, queryOptions);
            return Ok(new ContentContainer<PagedResult<OrderDTO>>(result, "Orders retrieved successfully."));
        }

        // GET: api/orders/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            try
            {
                var order = await _orderService.GetOrderByIdAsync(id);
                return Ok(new ContentContainer<OrderDTO>(order, "Order retrieved successfully."));
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex.Message);
                return NotFound(new ContentContainer<string>(null, ex.Message));
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDTO createOrderDto)
        {
            // Extract UserId and CustomerId from headers
            var userId = Request.Headers["UserId"].FirstOrDefault();
            var customerId = Request.Headers["CustomerId"].FirstOrDefault();

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(customerId))
            {
                return BadRequest(new ContentContainer<string>(null, "UserId and CustomerId must be provided in headers."));
            }

            // Set the CustomerId and UserId in DTO
            createOrderDto.CustomerId = int.Parse(customerId);

            if (!ModelState.IsValid)
                return BadRequest(new ContentContainer<string>(null, "Invalid order data."));

            var createdOrder = await _orderService.CreateOrderAsync(createOrderDto, userId);

            return CreatedAtAction(nameof(GetOrderById), new { id = createdOrder.Id }, new ContentContainer<OrderDTO>(createdOrder, "Order created successfully."));
        }

        // PUT: api/orders/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, [FromBody] OrderDTO orderDto)
        {
            if (!ModelState.IsValid)
            {
                var errorMessages = string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return BadRequest(new ContentContainer<string>(null, errorMessages));
            }

            try
            {
                var updatedOrder = await _orderService.UpdateOrderAsync(id, orderDto);
                return Ok(new ContentContainer<OrderDTO>(updatedOrder, "Order updated successfully."));
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex.Message);
                return NotFound(new ContentContainer<string>(null, ex.Message));
            }
        }

        // DELETE: api/orders/delete-multiple
        [HttpDelete("delete-multiple")]
        public async Task<IActionResult> DeleteMultipleOrders([FromForm] IEnumerable<int> ids)
        {
            var deletedCount = await _orderService.DeleteMultipleOrdersAsync(ids);

            if (deletedCount == 0)
                return NotFound(new ContentContainer<string>(null, "No matching orders found to delete."));

            return Ok(new ContentContainer<int>(deletedCount, $"{deletedCount} orders deleted successfully."));
        }

        // GET: api/orders/total/{id}
        [HttpGet("total/{id}")]
        public async Task<IActionResult> CalculateTotalOrderValue(int id)
        {
            var totalValue = await _orderService.CalculateTotalOrderValueAsync(id);
            return Ok(new ContentContainer<decimal>(totalValue, $"Total value for order {id} calculated successfully."));
        }
    }
}
