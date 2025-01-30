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
    public class PaymentService : IPaymentRepository
    {
        private readonly StoreContext _context;
        private readonly IMapper _mapper; 
        private readonly ILogger<PaymentService> _logger; 

        public PaymentService(StoreContext context, IMapper mapper, ILogger<PaymentService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

      
        public async Task<PagedResult<GetPaymentDTO>> GetAllPaymentsAsync(PaginationParameters paginationParameters, QueryOptions queryOptions)
        {
            try
            {
                var query = _context.Payments
                    .Include(p => p.Customer) 
                    .AsQueryable();

                if (!string.IsNullOrEmpty(queryOptions.Search))
                {
                    query = query.Where(p =>
                        p.Info.Contains(queryOptions.Search) ||
                        p.Amount.ToString().Contains(queryOptions.Search) ||
                        p.Customer.Name.Contains(queryOptions.Search)); // Search by customer name
                }

                // Apply sorting
                query = queryOptions.SortField.ToLower() switch
                {
                    "id" => queryOptions.SortDescending ? query.OrderByDescending(p => p.Id) : query.OrderBy(p => p.Id),
                    "customername" => queryOptions.SortDescending ? query.OrderByDescending(p => p.Customer.Name) : query.OrderBy(p => p.Customer.Name),
                    "amount" => queryOptions.SortDescending ? query.OrderByDescending(p => p.Amount) : query.OrderBy(p => p.Amount),
                    "date" => queryOptions.SortDescending ? query.OrderByDescending(p => p.Date) : query.OrderBy(p => p.Date),
                    _ => queryOptions.SortDescending ? query.OrderByDescending(p => p.Date) : query.OrderBy(p => p.Date) // Default sorting by Date
                };

                // Get total count after filtering
                var totalCount = await query.CountAsync();

                // Apply pagination
                var payments = await query
                    .Skip((paginationParameters.PageNumber - 1) * paginationParameters.PageSize)
                    .Take(paginationParameters.PageSize)
                    .ToListAsync();

                // Map to DTOs
                var paymentDtos = payments.Select(p => new GetPaymentDTO
                {
                    Id = p.Id,
                    Amount = p.Amount,
                    Date = p.Date,
                    Info = p.Info,
                    CustomerName = p.Customer.Name,
                    CustomerId = p.Customer.Id,

                }).ToList();

                return new PagedResult<GetPaymentDTO>
                {
                    Items = paymentDtos,
                    TotalCount = totalCount,
                    PageNumber = paginationParameters.PageNumber,
                    PageSize = paginationParameters.PageSize
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching all payments.");
                throw;
            }
        }




        public async Task<GetPaymentDTO> GetPaymentByIdAsync(int id)
        {
            try
            {
                var payment = await _context.Payments.FindAsync(id);
                if (payment == null)
                {
                    _logger.LogWarning($"Payment with ID {id} not found.");
                    return null; 
                }

                return _mapper.Map<GetPaymentDTO>(payment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while fetching the payment with ID {id}.");
                throw; 
            }
        }

      



        public async Task<PaymentDTO> CreatePaymentAsync(PaymentDTO paymentDto)
        {
            try
            {
                var payment = _mapper.Map<Payment>(paymentDto); // Map DTO to entity

                await _context.Payments.AddAsync(payment);
                await _context.SaveChangesAsync();

                // Update the MoneyOwed property of the customer
                var customer = await _context.Customers.FindAsync(payment.CustomerId);
                if (customer != null)
                {
                    customer.MoneyOwed = await _context.Payments
                        .Where(p => p.CustomerId == payment.CustomerId)
                        .SumAsync(p => p.Amount);

                    _context.Customers.Update(customer);
                    await _context.SaveChangesAsync();
                }

                paymentDto.Id = payment.Id; // Set the Id from the created payment
                return paymentDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a new payment.");
                throw; // Rethrow the exception to be handled by the caller
            }
        }


        public async Task<PaymentDTO> UpdatePaymentAsync(int id, PaymentDTO paymentDto)
        {
            try
            {
                var payment = await _context.Payments.FindAsync(id);
                if (payment == null)
                {
                    _logger.LogWarning($"Payment with ID {id} not found for update.");
                    return null; // Or throw a custom exception if needed
                }

                _mapper.Map(paymentDto, payment); // Update payment properties with DTO

                _context.Payments.Update(payment);
                await _context.SaveChangesAsync();

                return paymentDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while updating the payment with ID {id}.");
                throw; 
            }
        }

       
        public async Task<int> DeleteMultiplePaymentsAsync(IEnumerable<int> ids)
        {
            var payments = await _context.Payments
                .Where(p => ids.Contains(p.Id))
                .ToListAsync();

            if (!payments.Any())
            {
                _logger.LogWarning("No matching payments found for deletion.");
                return 0;
            }

            _context.Payments.RemoveRange(payments);
            var deletedCount = await _context.SaveChangesAsync();
            _logger.LogInformation($"{deletedCount} payments deleted successfully.");
            return deletedCount;
        }


        public async Task<IEnumerable<PaymentDTO>> GetPaymentsForCustomerAsync(int customerId)
        {
            try
            {
                var payments = await _context.Payments
                    .Where(p => p.CustomerId == customerId)
                    .ToListAsync();

                return _mapper.Map<List<PaymentDTO>>(payments); 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while fetching payments for customer ID {customerId}.");
                throw; 
            }
        }

        public async Task<IEnumerable<PaymentDTO>> GetPaymentsForOrderAsync(int orderId)
        {
            try
            {
                var payments = await _context.Payments
                    .Where(p => p.Id == orderId)
                    .ToListAsync();

                return _mapper.Map<List<PaymentDTO>>(payments); // Map entities to DTOs
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while fetching payments for order ID {orderId}.");
                throw; 
            }
        }

    }
}
