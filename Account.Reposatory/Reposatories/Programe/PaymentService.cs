using Account.Core.Dtos;
using Account.Core.Dtos.Program;
using Account.Core.Models.Entites;
using Account.Core.Services.Programe;
using Account.Reposatory.Data;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Reposatory.Reposatories.Programe
{
    public class PaymentService : IPaymentRepository
    {
        private readonly StoreContext _context; // Replace with your actual DbContext
        private readonly IMapper _mapper; // AutoMapper for mapping between entities and DTOs
        private readonly ILogger<PaymentService> _logger; // Logger for logging

        public PaymentService(StoreContext context, IMapper mapper, ILogger<PaymentService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }
        public async Task<PagedResult<PaymentDTO>> GetAllPaymentsAsync(PaginationParameters paginationParameters)
        {
            try
            {
                var query = _context.Payments.AsQueryable(); // Assuming Payments is your DbSet<Payment>

                // Apply pagination
                var totalItems = await query.CountAsync();
                var payments = await query
                    .Skip((paginationParameters.PageNumber - 1) * paginationParameters.PageSize)
                    .Take(paginationParameters.PageSize)
                    .ToListAsync();

                var paymentDtos = _mapper.Map<List<PaymentDTO>>(payments); // Map entities to DTOs
                return new PagedResult<PaymentDTO>
                {
                    Items = paymentDtos,
                    TotalCount = totalItems,
                    PageNumber = paginationParameters.PageNumber,
                    PageSize = paginationParameters.PageSize
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching all payments.");
                throw; // Rethrow the exception to be handled by the caller
            }
        }


        public async Task<PaymentDTO> GetPaymentByIdAsync(int id)
        {
            try
            {
                var payment = await _context.Payments.FindAsync(id);
                if (payment == null)
                {
                    _logger.LogWarning($"Payment with ID {id} not found.");
                    return null; // Or throw a custom exception if needed
                }

                return _mapper.Map<PaymentDTO>(payment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while fetching the payment with ID {id}.");
                throw; // Rethrow the exception to be handled by the caller
            }
        }

        public async Task<PaymentDTO> CreatePaymentAsync(PaymentDTO paymentDto)
        {
            try
            {
                var payment = _mapper.Map<Payment>(paymentDto); // Map DTO to entity

                await _context.Payments.AddAsync(payment);
                await _context.SaveChangesAsync();

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
                throw; // Rethrow the exception to be handled by the caller
            }
        }

        public async Task<bool> DeletePaymentAsync(int id)
        {
            try
            {
                var payment = await _context.Payments.FindAsync(id);
                if (payment == null)
                {
                    _logger.LogWarning($"Payment with ID {id} not found for deletion.");
                    return false; // Or throw a custom exception if needed
                }

                _context.Payments.Remove(payment);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while deleting the payment with ID {id}.");
                throw; // Rethrow the exception to be handled by the caller
            }
        }

        public async Task<IEnumerable<PaymentDTO>> GetPaymentsForCustomerAsync(int customerId)
        {
            try
            {
                var payments = await _context.Payments
                    .Where(p => p.CustomerId == customerId)
                    .ToListAsync();

                return _mapper.Map<List<PaymentDTO>>(payments); // Map entities to DTOs
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while fetching payments for customer ID {customerId}.");
                throw; // Rethrow the exception to be handled by the caller
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
                throw; // Rethrow the exception to be handled by the caller
            }
        }
    }
}
