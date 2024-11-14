using Account.Core.Dtos;
using Account.Core.Dtos.Program;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Core.Services.Programe
{
    public interface IPaymentRepository
    {
        Task<PagedResult<PaymentDTO>> GetAllPaymentsAsync(PaginationParameters paginationParameters);
        Task<PaymentDTO> GetPaymentByIdAsync(int id);
        Task<PaymentDTO> CreatePaymentAsync(PaymentDTO paymentDto);
        Task<PaymentDTO> UpdatePaymentAsync(int id, PaymentDTO paymentDto);
        Task<bool> DeletePaymentAsync(int id);
        Task<IEnumerable<PaymentDTO>> GetPaymentsForCustomerAsync(int customerId);
        Task<IEnumerable<PaymentDTO>> GetPaymentsForOrderAsync(int orderId);
    }
}
