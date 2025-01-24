using ShopSystem.Core.Dtos.Program;
using ShopSystem.Core.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopSystem.Core.Services.Programe
{
    public interface IPurchaseRepository
    {

        Task<PagedResult<GetPurchaseDTO>> GetAllPurchasesAsync(PaginationParameters paginationParameters, QueryOptions queryOptions);

        Task<GetPurchaseDTO> GetPurchaseByIdAsync(int purchaseId);
        Task<PurchaseDTO> CreatePurchaseAsync(CreatePurchaseDTO purchaseDto);
        Task<bool> UpdatePurchaseDetailsAsync(int purchaseId, CreatePurchaseDTO purchaseDto);
        Task<int> DeleteMultiplePurchasesAsync(IEnumerable<int> ids);

        Task<decimal> GetPurchaseTotalAmountAsync(int purchaseId);

    }
}
