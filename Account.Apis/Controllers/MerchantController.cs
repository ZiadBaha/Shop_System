using Account.Core.Dtos.Program;
using Account.Core.Dtos;
using Account.Core.Services.Programe;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Account.Core.Models;
using AutoMapper;

namespace Account.Apis.Controllers
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
        public async Task<ActionResult<PagedResult<MerchantDTO>>> GetAllMerchantsAsync([FromQuery] PaginationParameters paginationParameters, [FromQuery] QueryOptions queryOptions)
        {
            return Ok(await _merchantRepository.GetAllMerchantsAsync(paginationParameters, queryOptions));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MerchantDTO>> GetMerchantByIdAsync(int id)
        {
            try
            {
                return Ok(await _merchantRepository.GetMerchantByIdAsync(id));
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex.Message);
                return NotFound(ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<CreateMerchantDTO>> CreateMerchantAsync([FromBody] CreateMerchantDTO createMerchantDto)
        {
            var createdMerchant = await _merchantRepository.CreateMerchantAsync(createMerchantDto);
            return CreatedAtAction(nameof(GetMerchantByIdAsync), new { id = createdMerchant.Id }, createdMerchant);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<MerchantDTO>> UpdateMerchantAsync(int id, [FromBody] MerchantDTO merchantDto)
        {
            try
            {
                return Ok(await _merchantRepository.UpdateMerchantAsync(id, merchantDto));
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex.Message);
                return NotFound(ex.Message);
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


        [HttpGet("{merchantId}/balance")]
        public async Task<ActionResult<decimal>> CalculateOutstandingBalanceAsync(int merchantId)
        {
            return Ok(await _merchantRepository.CalculateOutstandingBalanceAsync(merchantId));
        }

        [HttpGet("{merchantId}/purchases")]
        public async Task<ActionResult<IEnumerable<PurchaseDTO>>> GetMerchantPurchasesAsync(int merchantId)
        {
            return Ok(await _merchantRepository.GetMerchantPurchasesAsync(merchantId));
        }
    }

}
