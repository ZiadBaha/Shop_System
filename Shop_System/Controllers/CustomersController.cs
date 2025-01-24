using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShopSystem.Core.Dtos.Program;
using ShopSystem.Core.Dtos;
using ShopSystem.Core.Models;
using ShopSystem.Core.Services.Programe;

namespace Shop_System.Controllers
{
    public class CustomersController : ApiBaseController
    {
        private readonly ICustomerRepository _customerService;
        private readonly ILogger<CustomersController> _logger;

        public CustomersController(ICustomerRepository customerService, ILogger<CustomersController> logger)
        {
            _customerService = customerService;
            _logger = logger;
        }


        /// Retrieves all customers with pagination, sorting, filtering, and search.
        [HttpGet]
        public async Task<IActionResult> GetAllCustomers([FromQuery] PaginationParameters paginationParameters, [FromQuery] QueryOptions queryOptions)
        {
            try
            {
                var customers = await _customerService.GetAllCustomersAsync(paginationParameters, queryOptions);
                return Ok(new { Message = "Customers retrieved successfully.", Data = customers });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve customers.");
                return StatusCode(500, new { Message = "An error occurred while retrieving customers." });
            }
        }



        /// Retrieves a specific customer by their ID.
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCustomerById(int id)
        {
            try
            {
                var customer = await _customerService.GetCustomerByIdAsync(id);
                return Ok(new { Message = "Customer retrieved successfully.", Data = customer });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, $"Customer with ID {id} not found.");
                return NotFound(new { Message = "Customer not found." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to retrieve customer with ID {id}.");
                return StatusCode(500, new { Message = "An error occurred while retrieving the customer." });
            }
        }




        [HttpPost]
        public async Task<IActionResult> CreateCustomer([FromBody] CreateCustomerDTO createCustomerDTO)
        {
            if (createCustomerDTO == null)
            {
                return BadRequest(new { Message = "Invalid customer data." });
            }

            try
            {
                var createdCustomer = await _customerService.CreateCustomerAsync(createCustomerDTO);
                return CreatedAtAction(nameof(GetCustomerById), new { id = createdCustomer.Id }, new { Message = "Customer created successfully.", Data = createdCustomer });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create customer.");
                return StatusCode(500, new { Message = "An error occurred while creating the customer." });
            }
        }


        /// Updates an existing customer.
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCustomer(int id, [FromBody] UpdateCustomerDTO updateCustomerDTO)
        {
            if (updateCustomerDTO == null)
            {
                return BadRequest(new { Message = "Invalid customer data." });
            }

            try
            {
                var updatedCustomer = await _customerService.UpdateCustomerAsync(id, updateCustomerDTO);
                return Ok(new { Message = "Customer updated successfully.", Data = updatedCustomer });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, $"Customer with ID {id} not found.");
                return NotFound(new { Message = "Customer not found." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to update customer with ID {id}.");
                return StatusCode(500, new { Message = "An error occurred while updating the customer." });
            }
        }

        // DELETE: api/customers/delete-multiple
        [HttpDelete("delete-multiple")]
        public async Task<IActionResult> DeleteMultipleCustomers([FromForm] IEnumerable<int> ids)
        {
            try
            {
                var deletedCount = await _customerService.DeleteMultipleCustomersAsync(ids);

                if (deletedCount == 0)
                    return NotFound(new ContentContainer<string>(null, "No matching customers found to delete."));

                return Ok(new ContentContainer<int>(deletedCount, $"{deletedCount} customers deleted successfully."));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting multiple customers.");
                return StatusCode(500, new ContentContainer<string>(null, "An error occurred while deleting the customers."));
            }
        }

        /// Calculates the debt of a specific customer by their ID.
        [HttpGet("{customerId}/debt")]
        public async Task<IActionResult> CalculateCustomerDebt(int customerId)
        {
            try
            {
                var debt = await _customerService.CalculateCustomerDebtAsync(customerId);
                return Ok(new { Message = "Customer debt calculated successfully.", Data = debt });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, $"Customer with ID {customerId} not found.");
                return NotFound(new { Message = "Customer not found." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to calculate debt for customer with ID {customerId}.");
                return StatusCode(500, new { Message = "An error occurred while calculating the customer's debt." });
            }
        }



       

        /// Retrieves all payments for a specific customer by their ID.
        [HttpGet("{customerId}/payments")]
        public async Task<IActionResult> GetCustomerPayments(int customerId)
        {
            Console.WriteLine($"Request to retrieve payments for Customer ID: {customerId}");

            try
            {
                var payments = await _customerService.GetCustomerPaymentsAsync(customerId);

                // Log the number of payments found
                Console.WriteLine($"Payments retrieved successfully for Customer ID: {customerId}. Found {payments.Count()} payments.");

                return Ok(new { Message = "Customer payments retrieved successfully.", Data = payments });
            }
            catch (KeyNotFoundException ex)
            {
                Console.WriteLine($"Customer with ID {customerId} not found.");

                _logger.LogWarning(ex, $"Customer with ID {customerId} not found.");
                return NotFound(new { Message = "Customer not found." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred while retrieving payments for Customer ID: {customerId}");

                _logger.LogError(ex, $"Failed to retrieve payments for customer with ID {customerId}.");
                return StatusCode(500, new { Message = "An error occurred while retrieving the customer's payments." });
            }
        }






    }



}
