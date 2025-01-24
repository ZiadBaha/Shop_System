using ShopSystem.Core.Dtos.Program;
using ShopSystem.Core.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopSystem.Core.Services.Programe
{
    public interface IPaymentRepository
    {
        Task<PagedResult<GetPaymentDTO>> GetAllPaymentsAsync(PaginationParameters paginationParameters, QueryOptions queryOptions);

        Task<GetPaymentDTO> GetPaymentByIdAsync(int id);
        Task<PaymentDTO> CreatePaymentAsync(PaymentDTO paymentDto);
        Task<PaymentDTO> UpdatePaymentAsync(int id, PaymentDTO paymentDto);

        Task<int> DeleteMultiplePaymentsAsync(IEnumerable<int> ids);
        Task<IEnumerable<PaymentDTO>> GetPaymentsForCustomerAsync(int customerId);
        Task<IEnumerable<PaymentDTO>> GetPaymentsForOrderAsync(int orderId);
    }
}
