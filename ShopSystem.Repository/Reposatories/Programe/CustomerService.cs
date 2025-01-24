using AutoMapper;
using Microsoft.Extensions.Logging;
using ShopSystem.Core.Dtos.Program;
using ShopSystem.Core.Dtos;
using ShopSystem.Core.Models.Entites;
using ShopSystem.Core.Services.Programe;
using ShopSystem.Repository.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ShopSystem.Repository.Reposatories.Programe
{
    public class CustomerService : ICustomerRepository
    {
        private readonly StoreContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<CustomerService> _logger;

        public CustomerService(StoreContext context, IMapper mapper, ILogger<CustomerService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }


        public async Task<PagedResult<CustomerDTO>> GetAllCustomersAsync(PaginationParameters paginationParameters, QueryOptions queryOptions)
        {
            var query = _context.Customers.AsQueryable();

            // Search by customer name
            if (!string.IsNullOrEmpty(queryOptions.Search))
            {
                query = query.Where(c => c.Name.Contains(queryOptions.Search));
            }

            // Filter by MoneyOwed range
            if (queryOptions.MinAmount.HasValue)
            {
                query = query.Where(c => c.MoneyOwed >= queryOptions.MinAmount.Value);
            }
            if (queryOptions.MaxAmount.HasValue)
            {
                query = query.Where(c => c.MoneyOwed <= queryOptions.MaxAmount.Value);
            }

            // Sorting
            query = queryOptions.SortField.ToLower() switch
            {
                "id" => queryOptions.SortDescending ? query.OrderByDescending(c => c.Id) : query.OrderBy(c => c.Id),
                "Name" => queryOptions.SortDescending ? query.OrderByDescending(c => c.Name) : query.OrderBy(c => c.Name),
                "moneyowed" => queryOptions.SortDescending ? query.OrderByDescending(c => c.MoneyOwed) : query.OrderBy(c => c.MoneyOwed),
                _ => queryOptions.SortDescending ? query.OrderByDescending(c => c.Name) : query.OrderBy(c => c.Name), // Default sorting by Name
            };

            // Get total count after filtering
            var totalCount = await query.CountAsync();

            // Apply pagination
            var customers = await query
                .Skip((paginationParameters.PageNumber - 1) * paginationParameters.PageSize)
                .Take(paginationParameters.PageSize)
                .ToListAsync();

            // Map to DTOs
            var customerDtos = _mapper.Map<IEnumerable<CustomerDTO>>(customers);

            return new PagedResult<CustomerDTO>
            {
                Items = customerDtos,
                TotalCount = totalCount,
                PageNumber = paginationParameters.PageNumber,
                PageSize = paginationParameters.PageSize
            };
        }


        public async Task<CustomerDTO> GetCustomerByIdAsync(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
                throw new KeyNotFoundException("Customer not found.");

            return _mapper.Map<CustomerDTO>(customer);
        }

        public async Task<CreateCustomerDTO> CreateCustomerAsync(CreateCustomerDTO createCustomerDTO)
        {
            var customerEntity = _mapper.Map<Customer>(createCustomerDTO);
            await _context.Customers.AddAsync(customerEntity);
            await _context.SaveChangesAsync();
            return _mapper.Map<CreateCustomerDTO>(customerEntity);
        }



        public async Task<UpdateCustomerDTO> UpdateCustomerAsync(int id, UpdateCustomerDTO updateCustomerDTO)
        {
            var existingCustomer = await _context.Customers.FindAsync(id);
            if (existingCustomer == null)
                throw new KeyNotFoundException("Customer not found.");

            _mapper.Map(updateCustomerDTO, existingCustomer); // Maps the changes to the entity
            _context.Customers.Update(existingCustomer);
            await _context.SaveChangesAsync();

            return _mapper.Map<UpdateCustomerDTO>(existingCustomer);
        }



        public async Task<int> DeleteMultipleCustomersAsync(IEnumerable<int> ids)
        {
            var customersToDelete = await _context.Customers
                .Where(c => ids.Contains(c.Id))
                .ToListAsync();

            if (!customersToDelete.Any())
            {
                _logger.LogWarning("No matching customers found for bulk deletion.");
                return 0;
            }

            _context.Customers.RemoveRange(customersToDelete);
            await _context.SaveChangesAsync();

            return customersToDelete.Count;
        }

        public async Task<decimal?> CalculateCustomerDebtAsync(int customerId)
        {
            // Assuming you have a relationship where an order has a total amount
            var totalDebt = await _context.Orders
                .Where(o => o.CustomerId == customerId)
                .SumAsync(o => o.TotalAmount);

            return totalDebt;
        }

      
       
        public async Task<IEnumerable<PaymentDTO>> GetCustomerPaymentsAsync(int customerId)
        {
            var customerExists = await _context.Customers.AnyAsync(c => c.Id == customerId);

            if (!customerExists)
            {
                throw new KeyNotFoundException("Customer not found.");
            }

            var payments = await _context.Payments
                                          .Where(p => p.CustomerId == customerId)
                                          .Select(p => new PaymentDTO
                                          {
                                              Id = p.Id,
                                              CustomerId = p.CustomerId,
                                              Amount = p.Amount,
                                              Date = p.Date,
                                              Info = p.Info
                                          })
                                          .ToListAsync();

            Console.WriteLine($"Found {payments.Count} payments for Customer ID: {customerId}");
            return payments;
        }

    }
}
