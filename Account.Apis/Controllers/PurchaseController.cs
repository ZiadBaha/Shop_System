using Account.Core.Dtos.Program;
using Account.Core.Dtos;
using Account.Core.Services.Programe;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Account.Apis.Controllers
{

    public class PurchaseController : ApiBaseController
    {
        private readonly IPurchaseRepository _purchaseRepository;
        private readonly ILogger<PurchaseController> _logger;

        public PurchaseController(IPurchaseRepository purchaseRepository, ILogger<PurchaseController> logger)
        {
            _purchaseRepository = purchaseRepository;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<PurchaseDTO>>> GetAllPurchasesAsync([FromQuery] PaginationParameters paginationParameters, [FromQuery] QueryOptions queryOptions)
        {
            try
            {
                var result = await _purchaseRepository.GetAllPurchasesAsync(paginationParameters, queryOptions);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching purchases.");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PurchaseDTO>> GetPurchaseByIdAsync(int id)
        {
            try
            {
                var purchase = await _purchaseRepository.GetPurchaseByIdAsync(id);
                return Ok(purchase);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Purchase with ID {id} not found.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while fetching the purchase with ID {id}.");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<ActionResult<PurchaseDTO>> CreatePurchaseAsync([FromBody] PurchaseDTO purchaseDto)
        {
            try
            {
                var purchase = await _purchaseRepository.CreatePurchaseAsync(purchaseDto);
                return CreatedAtAction(nameof(GetPurchaseByIdAsync), new { id = purchase.Id }, purchase);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the purchase.");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<PurchaseDTO>> UpdatePurchaseAsync(int id, [FromBody] PurchaseDTO purchaseDto)
        {
            try
            {
                var updatedPurchase = await _purchaseRepository.UpdatePurchaseAsync(id, purchaseDto);
                return Ok(updatedPurchase);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Purchase with ID {id} not found.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while updating the purchase with ID {id}.");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePurchaseAsync(int id, [FromQuery] bool isReturn = false)
        {
            try
            {
                var success = await _purchaseRepository.DeletePurchaseAsync(id, isReturn);
                if (!success)
                {
                    return NotFound($"Purchase with ID {id} not found.");
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while deleting the purchase with ID {id}.");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{purchaseId}/products")]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProductsInPurchaseAsync(int purchaseId)
        {
            try
            {
                var products = await _purchaseRepository.GetProductsInPurchaseAsync(purchaseId);
                return Ok(products);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Purchase with ID {purchaseId} not found.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while fetching products for purchase ID {purchaseId}.");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{purchaseId}/total")]
        public async Task<ActionResult<decimal>> CalculateTotalPurchaseAmountAsync(int purchaseId)
        {
            try
            {
                var totalAmount = await _purchaseRepository.CalculateTotalPurchaseAmountAsync(purchaseId);
                return Ok(totalAmount);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Purchase with ID {purchaseId} not found.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while calculating the total amount for purchase ID {purchaseId}.");
                return StatusCode(500, "Internal server error");
            }
        }
    }
} 
