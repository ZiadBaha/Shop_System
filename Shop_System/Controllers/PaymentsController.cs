using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShopSystem.Core.Dtos.Program;
using ShopSystem.Core.Dtos;
using ShopSystem.Core.Models;
using ShopSystem.Core.Services.Programe;

namespace Shop_System.Controllers
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

        // GET: api/payments
        [HttpGet]
        public async Task<IActionResult> GetAllPayments([FromQuery] PaginationParameters paginationParameters, [FromQuery] QueryOptions queryOptions)
        {
            try
            {
                var payments = await _paymentService.GetAllPaymentsAsync(paginationParameters, queryOptions);

                return Ok(new ContentContainer<PagedResult<GetPaymentDTO>>(payments, "Payments retrieved successfully."));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving payments.");
                return StatusCode(500, new ContentContainer<string>(null, "An error occurred while retrieving payments."));
            }
        }



        ///// Retrieves a specific payment by ID.
        //[HttpGet("{id:int}")]
        //public async Task<IActionResult> GetPaymentById(int id)
        //{
        //    try
        //    {
        //        var payment = await _paymentService.GetPaymentByIdAsync(id);
        //        if (payment == null)
        //            return NotFound(new { Message = $"Payment with ID {id} not found.", Result = (object)null });

        //        return Ok(new { Message = "Payment retrieved successfully.", Result = new { Data = payment } });
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, $"An error occurred while retrieving payment with ID {id}.");
        //        return StatusCode(500, new { Message = "An error occurred while retrieving the payment.", Result = (object)null });
        //    }
        //}

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetPaymentById(int id)
        {
            try
            {
                var payment = await _paymentService.GetPaymentByIdAsync(id);
                if (payment == null)
                    return NotFound(new ContentContainer<string>(null, $"Payment with ID {id} not found."));

                return Ok(new ContentContainer<GetPaymentDTO>(payment, "Payment retrieved successfully."));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving payment with ID {id}.");
                return StatusCode(500, new ContentContainer<string>(null, "An error occurred while retrieving the payment."));
            }
        }



        [HttpPost]
        public async Task<ActionResult<ContentContainer<PaymentDTO>>> CreatePayment([FromBody] PaymentDTO paymentDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ContentContainer<string>(
                    null,
                    "Invalid payment data. Please check the input fields and try again."
                ));
            }

            try
            {
                var createdPayment = await _paymentService.CreatePaymentAsync(paymentDto);
                return CreatedAtAction(
                    nameof(GetPaymentById),
                    new { id = createdPayment.Id },
                    new ContentContainer<PaymentDTO>(createdPayment, "Payment created successfully.")
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a new payment.");
                return StatusCode(500, new ContentContainer<string>(
                    null,
                    "An error occurred while creating the payment. Please try again later."
                ));
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

        // DELETE: api/payments/delete-multiple
        [HttpDelete("delete-multiple")]
        public async Task<IActionResult> DeleteMultiplePayments([FromForm] IEnumerable<int> ids)
        {
            try
            {
                var deletedCount = await _paymentService.DeleteMultiplePaymentsAsync(ids);

                if (deletedCount == 0)
                    return NotFound(new ContentContainer<string>(null, "No matching payments found to delete."));

                return Ok(new ContentContainer<int>(deletedCount, $"{deletedCount} payments deleted successfully."));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting multiple payments.");
                return StatusCode(500, new ContentContainer<string>(null, "An error occurred while processing your request."));
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
