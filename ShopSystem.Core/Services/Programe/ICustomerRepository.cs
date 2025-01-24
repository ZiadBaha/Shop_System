using ShopSystem.Core.Dtos.Program;
using ShopSystem.Core.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopSystem.Core.Services.Programe
{
    public interface ICustomerRepository
    {
        Task<PagedResult<CustomerDTO>> GetAllCustomersAsync(PaginationParameters paginationParameters, QueryOptions queryOptions);
        Task<CustomerDTO> GetCustomerByIdAsync(int id);
        Task<CreateCustomerDTO> CreateCustomerAsync(CreateCustomerDTO createCustomerDTO);
        Task<UpdateCustomerDTO> UpdateCustomerAsync(int id, UpdateCustomerDTO updateCustomerDTO);
        Task<int> DeleteMultipleCustomersAsync(IEnumerable<int> ids);
        Task<decimal?> CalculateCustomerDebtAsync(int customerId);
        Task<IEnumerable<PaymentDTO>> GetCustomerPaymentsAsync(int customerId);
    }
}
