using Account.Core.Dtos.Program;
using Account.Core.Dtos;
using Account.Core.Models.Entites;
using Account.Core.Services.Programe;
using AutoMapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Account.Reposatory.Data;
using Microsoft.EntityFrameworkCore;
using Account.Core.Enums;

namespace Account.Reposatory.Reposatories.Programe
{
    public class OrderService : IOrderRepository
    {
        private readonly StoreContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<OrderService> _logger;

        public OrderService(StoreContext context, IMapper mapper, ILogger<OrderService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PagedResult<OrderDTO>> GetAllOrdersAsync(PaginationParameters paginationParameters, QueryOptions queryOptions)
        {
            var query = _context.Orders.AsQueryable();

            // Filtering based on query options
            if (!string.IsNullOrEmpty(queryOptions.Search))
            {
                query = query.Where(o => o.Customer.Name.Contains(queryOptions.Search)); // Assuming Customer has Name property
            }

            // Sorting based on query options
            if (!string.IsNullOrEmpty(queryOptions.SortField))
            {
                var propertyInfo = typeof(Order).GetProperty(queryOptions.SortField);
                if (propertyInfo != null)
                {
                    query = queryOptions.SortDescending
                        ? query.OrderByDescending(o => EF.Property<object>(o, queryOptions.SortField))
                        : query.OrderBy(o => EF.Property<object>(o, queryOptions.SortField));
                }
            }

            // Total count for pagination
            var totalCount = await query.CountAsync();

            // Pagination
            var orders = await query
                .Skip((paginationParameters.PageNumber - 1) * paginationParameters.PageSize)
                .Take(paginationParameters.PageSize)
                .ToListAsync();

            var orderDtos = _mapper.Map<IEnumerable<OrderDTO>>(orders);

            return new PagedResult<OrderDTO>
            {
                Items = orderDtos,
                TotalCount = totalCount,
                PageNumber = paginationParameters.PageNumber,
                PageSize = paginationParameters.PageSize
            };
        }

        public async Task<OrderDTO> GetOrderByIdAsync(int id)
        {
            var order = await _context.Orders.Include(o => o.OrderItems) // Including products for order details
                .FirstOrDefaultAsync(o => o.Id == id);
            if (order == null)
            {
                _logger.LogWarning($"Order with ID {id} not found.");
                throw new KeyNotFoundException("Order not found.");
            }

            return _mapper.Map<OrderDTO>(order);
        }

        public async Task<OrderDTO> CreateOrderAsync(CreateOrderDTO createOrderDto, string userId)
        {
            // Map DTO to Order entity
            var orderEntity = _mapper.Map<Order>(createOrderDto);

            // Set the UserId (Employee/Admin) who is creating the order
            orderEntity.UserId = userId;

            // Set the CustomerId for the order
            orderEntity.CustomerId = createOrderDto.CustomerId;

            // Add order items
            foreach (var orderItemDto in createOrderDto.OrderItems)
            {
                var orderItem = _mapper.Map<OrderItem>(orderItemDto);
                orderEntity.OrderItems.Add(orderItem);
            }

            // Save to database
            await _context.Orders.AddAsync(orderEntity);
            await _context.SaveChangesAsync();

            // Return the DTO after saving
            return _mapper.Map<OrderDTO>(orderEntity);
        }

        public async Task<OrderDTO> UpdateOrderAsync(int id, OrderDTO orderDto)
        {
            var existingOrder = await _context.Orders.FindAsync(id);
            if (existingOrder == null)
            {
                _logger.LogWarning($"Order with ID {id} not found for update.");
                throw new KeyNotFoundException("Order not found.");
            }

            _mapper.Map(orderDto, existingOrder); // Maps updates to the existing entity
            _context.Orders.Update(existingOrder);
            await _context.SaveChangesAsync();

            return _mapper.Map<OrderDTO>(existingOrder);
        }

        public async Task<int> DeleteMultipleOrdersAsync(IEnumerable<int> ids)
        {
            var ordersToDelete = await _context.Orders
                .Where(o => ids.Contains(o.Id))
                .ToListAsync();

            if (!ordersToDelete.Any())
            {
                _logger.LogWarning("No matching orders found for bulk deletion.");
                return 0;
            }

            _context.Orders.RemoveRange(ordersToDelete);
            await _context.SaveChangesAsync();

            return ordersToDelete.Count;
        }

        public async Task<decimal> CalculateTotalOrderValueAsync(int id)
        {
            var order = await _context.Orders.Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == id);
            if (order == null)
            {
                _logger.LogWarning($"Order with ID {id} not found for total calculation.");
                throw new KeyNotFoundException("Order not found.");
            }

            var totalValue = order.OrderItems.Sum(p => p.TotalPrice); // Calculate total price from order items
            return totalValue;
        }
    }
}
