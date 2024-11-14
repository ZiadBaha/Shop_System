using Account.Core.Dtos;
using Account.Core.Dtos.Program;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Core.Services.Programe
{
    public interface IMerchantRepository
    {
        //Task<IEnumerable<MerchantDTO>> GetAllMerchantsAsync();
        Task<PagedResult<MerchantDTO>> GetAllMerchantsAsync(PaginationParameters paginationParameters, QueryOptions queryOptions);
        Task<MerchantDTO> GetMerchantByIdAsync(int id);
        Task<CreateMerchantDTO> CreateMerchantAsync(CreateMerchantDTO createMerchantDTO);
        Task<MerchantDTO> UpdateMerchantAsync(int id, MerchantDTO merchantDto);
        Task<(int deletedCount, string message)> DeleteMultipleMerchantsAsync(IEnumerable<int> ids);
        Task<decimal> CalculateOutstandingBalanceAsync(int merchantId);
        Task<IEnumerable<PurchaseDTO>> GetMerchantPurchasesAsync(int merchantId);
       
    }
}
