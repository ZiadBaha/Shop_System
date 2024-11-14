using Account.Core.Dtos.Program;
using Account.Core.Dtos;
using Account.Reposatory.Reposatories.Programe;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Account.Core.Services.Programe;
namespace Account.Apis.Controllers
{
   
    public class PaymentsController : ApiBaseController
    {
        private readonly IPaymentRepository _paymentService;
        private readonly ILogger<PaymentsController> _logger;

        public PaymentsController(IPaymentRepository paymentService, ILogger<PaymentsController> logger)
        {
            _paymentService = paymentService;
            _logger = logger;
        }

        /// Retrieves a paginated list of payments.
        [HttpGet]
        public async Task<IActionResult> GetAllPayments([FromQuery] PaginationParameters paginationParameters)
        {
            try
            {
                var payments = await _paymentService.GetAllPaymentsAsync(paginationParameters);
                return Ok(new { Message = "Payments retrieved successfully.", Result = new { Data = payments } });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving payments.");
                return StatusCode(500, new { Message = "An error occurred while retrieving payments.", Result = (object)null });
            }
        }

        /// Retrieves a specific payment by ID.
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetPaymentById(int id)
        {
            try
            {
                var payment = await _paymentService.GetPaymentByIdAsync(id);
                if (payment == null)
                    return NotFound(new { Message = $"Payment with ID {id} not found.", Result = (object)null });

                return Ok(new { Message = "Payment retrieved successfully.", Result = new { Data = payment } });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving payment with ID {id}.");
                return StatusCode(500, new { Message = "An error occurred while retrieving the payment.", Result = (object)null });
            }
        }

        /// Creates a new payment.
        [HttpPost]
        public async Task<IActionResult> CreatePayment([FromBody] PaymentDTO paymentDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { Message = "Invalid payment data.", Result = new { Errors = ModelState.Values } });

            try
            {
                var createdPayment = await _paymentService.CreatePaymentAsync(paymentDto);
                return CreatedAtAction(nameof(GetPaymentById), new { id = createdPayment.Id },
                    new { Message = "Payment created successfully.", Result = new { Data = createdPayment } });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a new payment.");
                return StatusCode(500, new { Message = "An error occurred while creating the payment.", Result = (object)null });
            }
        }

        /// Updates an existing payment.
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdatePayment(int id, [FromBody] PaymentDTO paymentDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { Message = "Invalid payment data.", Result = new { Errors = ModelState.Values } });

            try
            {
                var updatedPayment = await _paymentService.UpdatePaymentAsync(id, paymentDto);
                if (updatedPayment == null)
                    return NotFound(new { Message = $"Payment with ID {id} not found for update.", Result = (object)null });

                return Ok(new { Message = "Payment updated successfully.", Result = new { Data = updatedPayment } });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while updating payment with ID {id}.");
                return StatusCode(500, new { Message = "An error occurred while updating the payment.", Result = (object)null });
            }
        }

        /// Deletes a payment by ID.
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeletePayment(int id)
        {
            try
            {
                var success = await _paymentService.DeletePaymentAsync(id);
                if (!success)
                    return NotFound(new { Message = $"Payment with ID {id} not found for deletion.", Result = (object)null });

                return Ok(new { Message = "Payment deleted successfully.", Result = (object)null });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while deleting payment with ID {id}.");
                return StatusCode(500, new { Message = "An error occurred while deleting the payment.", Result = (object)null });
            }
        }

        /// Retrieves payments for a specific customer.
        [HttpGet("customer/{customerId:int}")]
        public async Task<IActionResult> GetPaymentsForCustomer(int customerId)
        {
            try
            {
                var payments = await _paymentService.GetPaymentsForCustomerAsync(customerId);
                return Ok(new { Message = "Payments for the specified customer retrieved successfully.", Result = new { Data = payments } });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving payments for customer ID {customerId}.");
                return StatusCode(500, new { Message = "An error occurred while retrieving payments for the customer.", Result = (object)null });
            }
        }

        /// Retrieves payments for a specific order.
        [HttpGet("order/{orderId:int}")]
        public async Task<IActionResult> GetPaymentsForOrder(int orderId)
        {
            try
            {
                var payments = await _paymentService.GetPaymentsForOrderAsync(orderId);
                return Ok(new { Message = "Payments for the specified order retrieved successfully.", Result = new { Data = payments } });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving payments for order ID {orderId}.");
                return StatusCode(500, new { Message = "An error occurred while retrieving payments for the order.", Result = (object)null });
            }
        }
    }
}
