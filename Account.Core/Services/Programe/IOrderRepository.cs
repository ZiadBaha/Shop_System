using Account.Core.Dtos;
using Account.Core.Dtos.Program;
using Account.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Core.Services.Programe
{
    public interface IOrderRepository
    {
        Task<PagedResult<OrderDTO>> GetAllOrdersAsync(PaginationParameters paginationParameters, QueryOptions queryOptions);
        Task<OrderDTO> GetOrderByIdAsync(int id);
        Task<OrderDTO> CreateOrderAsync(CreateOrderDTO createOrderDto, string userId);  // Method to create an order
        Task<OrderDTO> UpdateOrderAsync(int id, OrderDTO orderDto);
        Task<int> DeleteMultipleOrdersAsync(IEnumerable<int> ids);
        Task<decimal> CalculateTotalOrderValueAsync(int id);
    }
}
