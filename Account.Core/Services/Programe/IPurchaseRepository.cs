using Account.Core.Dtos;
using Account.Core.Dtos.Program;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Core.Services.Programe
{
    public interface IPurchaseRepository
    {
        // Task<IEnumerable<PurchaseDTO>> GetAllPurchasesAsync();
        Task<PagedResult<PurchaseDTO>> GetAllPurchasesAsync(PaginationParameters paginationParameters, QueryOptions queryOptions);

        //Task<PagedResult<PurchaseDTO>> GetAllPurchasesAsync(PaginationParameters paginationParameters);
        Task<PurchaseDTO> GetPurchaseByIdAsync(int id);
        //Task<PurchaseDTO> CreatePurchaseAsync(PurchaseDTO purchaseDto);
        Task<PurchaseDTO> CreatePurchaseAsync(PurchaseDTO purchaseDto, bool isReturn = false);

        Task<PurchaseDTO> UpdatePurchaseAsync(int id, PurchaseDTO purchaseDto);
        //Task<bool> DeletePurchaseAsync(int id);

        Task<bool> DeletePurchaseAsync(int id, bool isReturn = false);

        Task<IEnumerable<ProductDTO>> GetProductsInPurchaseAsync(int purchaseId);
        Task<decimal> CalculateTotalPurchaseAmountAsync(int purchaseId);
    }
}
