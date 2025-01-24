using ShopSystem.Core.Dtos.Program;
using ShopSystem.Core.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopSystem.Core.Services.Programe
{
    public interface IMerchantRepository
    {
        //Task<IEnumerable<MerchantDTO>> GetAllMerchantsAsync();
        Task<PagedResult<MerchantDTO>> GetAllMerchantsAsync(PaginationParameters paginationParameters, QueryOptions queryOptions);
        Task<MerchantDTO> GetMerchantByIdAsync(int id);
        Task<CreateMerchantDTO> CreateMerchantAsync(CreateMerchantDTO createMerchantDTO);
        Task<UpdateMerchantDTO> UpdateMerchantAsync(int id, UpdateMerchantDTO updateMerchantDTO);
        Task<(int deletedCount, string message)> DeleteMultipleMerchantsAsync(IEnumerable<int> ids);
        //Task<decimal> CalculateOutstandingBalanceAsync(int merchantId);
        Task<IEnumerable<PurchaseDTO>> GetMerchantPurchasesAsync(int merchantId);
        Task<bool> IsPhoneOrNameDuplicatedAsync(string phone, string name);
    }
}
