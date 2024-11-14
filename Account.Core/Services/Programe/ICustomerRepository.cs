using Account.Core.Dtos;
using Account.Core.Dtos.Program;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Core.Services.Programe
{
    public interface ICustomerRepository
    {
        //Task<PagedResult<CustomerDTO>> GetAllCustomersAsync(PaginationParameters paginationParameters);
        Task<PagedResult<CustomerDTO>> GetAllCustomersAsync(PaginationParameters paginationParameters, QueryOptions queryOptions);

        Task<CustomerDTO> GetCustomerByIdAsync(int id);
        Task<CreateCustomerDTO> CreateCustomerAsync(CreateCustomerDTO createCustomerDTO);
        Task<UpdateCustomerDTO> UpdateCustomerAsync(int id, UpdateCustomerDTO updateCustomerDTO);
        //Task<bool> DeleteCustomerAsync(int id);
        Task<int> DeleteMultipleCustomersAsync(IEnumerable<int> ids);

        Task<decimal?> CalculateCustomerDebtAsync(int customerId);
        Task<IEnumerable<OrderDTO>> GetCustomerOrdersAsync(int customerId);
        Task<IEnumerable<PaymentDTO>> GetCustomerPaymentsAsync(int customerId);

    }
}
