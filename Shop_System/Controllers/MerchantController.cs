using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShopSystem.Core.Dtos.Program;
using ShopSystem.Core.Dtos;
using ShopSystem.Core.Models;
using ShopSystem.Core.Services.Programe;

namespace Shop_System.Controllers
{
    public class MerchantController : ApiBaseController
    {
        private readonly IMerchantRepository _merchantRepository;
        private readonly ILogger<MerchantController> _logger;

        public MerchantController(IMerchantRepository merchantRepository, ILogger<MerchantController> logger)
        {
            _merchantRepository = merchantRepository;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<ContentContainer<PagedResult<MerchantDTO>>>> GetAllMerchantsAsync([FromQuery] PaginationParameters paginationParameters, [FromQuery] QueryOptions queryOptions)
        {
            var pagedMerchants = await _merchantRepository.GetAllMerchantsAsync(paginationParameters, queryOptions);
            return Ok(new ContentContainer<PagedResult<MerchantDTO>>(pagedMerchants, "Merchants retrieved successfully."));
        }

        #region Get By Id
        [HttpGet("{id}")]
        public async Task<ActionResult<ContentContainer<MerchantDTO>>> GetMerchantByIdAsync(int id)
        {
            try
            {
                var merchant = await _merchantRepository.GetMerchantByIdAsync(id);
                if (merchant == null)
                {
                    return NotFound(new ContentContainer<MerchantDTO>(null, $"Merchant with ID {id} not found."));
                }

                return Ok(new ContentContainer<MerchantDTO>(merchant, "Merchant retrieved successfully."));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving merchant with ID {id}");
                return StatusCode(500, new ContentContainer<MerchantDTO>(null, "An error occurred while retrieving the merchant."));
            }
        }
        #endregion

        [HttpPost]
        public async Task<ActionResult<ContentContainer<CreateMerchantDTO>>> CreateMerchantAsync([FromBody] CreateMerchantDTO createMerchantDto)
        {
            if (await _merchantRepository.IsPhoneOrNameDuplicatedAsync(createMerchantDto.Phone, createMerchantDto.Name))
            {
                return BadRequest(new ContentContainer<string>(null, "Duplicate phone number or name. Each merchant must have a unique phone number and name."));
            }

            var createdMerchant = await _merchantRepository.CreateMerchantAsync(createMerchantDto);
            return Created("", new ContentContainer<CreateMerchantDTO>(createdMerchant, "Merchant created successfully."));
        }
        [HttpPut("{id}")]
        public async Task<ActionResult<ContentContainer<UpdateMerchantDTO>>> UpdateMerchantAsync(int id, [FromBody] UpdateMerchantDTO merchantDto)
        {
            try
            {
                var updatedMerchant = await _merchantRepository.UpdateMerchantAsync(id, merchantDto);
                return Ok(new ContentContainer<UpdateMerchantDTO>(updatedMerchant, "Merchant updated successfully."));
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex.Message);
                return NotFound(new ContentContainer<string>(null, ex.Message));
            }
        }

        // DELETE: api/merchants/delete-multiple
        [HttpDelete("delete-multiple")]
        public async Task<IActionResult> DeleteMultipleMerchants([FromForm] IEnumerable<int> ids)
        {
            try
            {
                var (deletedCount, message) = await _merchantRepository.DeleteMultipleMerchantsAsync(ids);

                if (deletedCount == 0)
                    return BadRequest(new ContentContainer<string>(null, message));

                return Ok(new ContentContainer<int>(deletedCount, message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting multiple merchants.");
                return StatusCode(500, new ContentContainer<string>(null, "An error occurred while processing your request."));
            }
        }

        //[HttpGet("{merchantId}/balance")]
        //public async Task<ActionResult<ContentContainer<decimal>>> CalculateOutstandingBalanceAsync(int merchantId)
        //{
        //    var balance = await _merchantRepository.CalculateOutstandingBalanceAsync(merchantId);
        //    return Ok(new ContentContainer<decimal>(balance, "Outstanding balance calculated successfully."));
        //}

        [HttpGet("{merchantId}/purchases")]
        public async Task<ActionResult<ContentContainer<IEnumerable<PurchaseDTO>>>> GetMerchantPurchasesAsync(int merchantId)
        {
            var purchases = await _merchantRepository.GetMerchantPurchasesAsync(merchantId);
            return Ok(new ContentContainer<IEnumerable<PurchaseDTO>>(purchases, "Merchant purchases retrieved successfully."));
        }
    }

}
