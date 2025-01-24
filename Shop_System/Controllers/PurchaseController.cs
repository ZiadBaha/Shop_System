using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShopSystem.Core.Dtos.Program;
using ShopSystem.Core.Dtos;
using ShopSystem.Core.Models;
using ShopSystem.Core.Services.Programe;
using Newtonsoft.Json;
using AutoMapper;
using ShopSystem.Repository.Reposatories.Programe;

namespace Shop_System.Controllers
{
    public class PurchaseController : ApiBaseController
    {
        private readonly IPurchaseRepository _purchaseService;
        private readonly ILogger<PurchaseController> _logger;

        public PurchaseController(IPurchaseRepository purchaseService, ILogger<PurchaseController> logger)
        {
            _purchaseService = purchaseService;
            _logger = logger;
        }

        // GET: api/purchases
        [HttpGet]
        public async Task<IActionResult> GetAllPurchases([FromQuery] PaginationParameters paginationParameters, [FromQuery] QueryOptions queryOptions)
        {
            try
            {
                var purchases = await _purchaseService.GetAllPurchasesAsync(paginationParameters, queryOptions);
                return Ok(new ContentContainer<object>(purchases, "Purchases retrieved successfully."));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving purchases.");
                return StatusCode(500, new ContentContainer<string>(null, "An error occurred while processing your request."));
            }
        }

        // GET: api/purchases/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPurchaseById(int id)
        {
            try
            {
                var purchase = await _purchaseService.GetPurchaseByIdAsync(id);
                if (purchase == null)
                    return NotFound(new ContentContainer<string>(null, "Purchase not found."));

                return Ok(new ContentContainer<object>(purchase, "Purchase retrieved successfully."));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving purchase with ID {id}.");
                return StatusCode(500, new ContentContainer<string>(null, "An error occurred while processing your request."));
            }
        }

        // POST: api/purchases
        [HttpPost]
        public async Task<IActionResult> CreatePurchase([FromBody] CreatePurchaseDTO purchaseDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ContentContainer<string>(null, "Invalid request data."));

            try
            {
                var createdPurchase = await _purchaseService.CreatePurchaseAsync(purchaseDto);
                return CreatedAtAction(nameof(GetPurchaseById), new { id = createdPurchase.Id },
                    new ContentContainer<object>(createdPurchase, "Purchase created successfully."));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the purchase.");
                return StatusCode(500, new ContentContainer<string>(null, "An error occurred while processing your request."));
            }
        }

        // PUT: api/purchases/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePurchase(int id, [FromBody] CreatePurchaseDTO purchaseDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ContentContainer<string>(null, "Invalid request data."));

            try
            {
                var isUpdated = await _purchaseService.UpdatePurchaseDetailsAsync(id, purchaseDto);
                if (!isUpdated)
                    return NotFound(new ContentContainer<string>(null, "Purchase not found for update."));

                return Ok(new ContentContainer<string>(null, "Purchase updated successfully."));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while updating purchase with ID {id}.");
                return StatusCode(500, new ContentContainer<string>(null, "An error occurred while processing your request."));
            }
        }

        // DELETE: api/purchases/delete-multiple
        [HttpDelete("delete-multiple")]
        public async Task<IActionResult> DeleteMultiplePurchases([FromForm] IEnumerable<int> ids)
        {
            if (ids == null || !ids.Any())
                return BadRequest(new ContentContainer<string>(null, "No purchase IDs provided for deletion."));

            try
            {
                var deletedCount = await _purchaseService.DeleteMultiplePurchasesAsync(ids);

                if (deletedCount == 0)
                    return NotFound(new ContentContainer<string>(null, "No matching purchases found to delete."));

                return Ok(new ContentContainer<int>(deletedCount, $"{deletedCount} purchases deleted successfully."));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting multiple purchases.");
                return StatusCode(500, new ContentContainer<string>(null, "An error occurred while processing your request."));
            }
        }



        // GET: api/purchases/{purchaseId}/total-amount
        [HttpGet("{purchaseId}/total-amount")]
        public async Task<IActionResult> GetPurchaseTotalAmount(int purchaseId)
        {
            try
            {
                var totalAmount = await _purchaseService.GetPurchaseTotalAmountAsync(purchaseId);

                if (totalAmount == 0)
                {
                    return NotFound(new ContentContainer<string>(null, $"Purchase with ID {purchaseId} not found."));
                }

                return Ok(new ContentContainer<decimal>(totalAmount, "Purchase total amount calculated successfully."));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while calculating the total amount for purchase ID {purchaseId}.");
                return StatusCode(500, new ContentContainer<string>(null, "An error occurred while processing your request."));
            }
        }


    }
}
