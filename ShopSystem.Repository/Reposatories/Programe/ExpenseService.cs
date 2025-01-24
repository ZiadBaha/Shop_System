using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ShopSystem.Core.Dtos.Program;
using ShopSystem.Core.Dtos;
using ShopSystem.Core.Enums;
using ShopSystem.Core.Models.Entites;
using ShopSystem.Core.Services.Programe;
using ShopSystem.Repository.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopSystem.Repository.Reposatories.Programe
{
    public class ExpenseService : IExpenseRepository
    {
        private readonly StoreContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<ExpenseService> _logger;

        public ExpenseService(StoreContext context, IMapper mapper, ILogger<ExpenseService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PagedResult<ExpenseDTO>> GetAllExpensesAsync(PaginationParameters paginationParameters, QueryOptions queryOptions)
        {
            var query = _context.Expenses.AsQueryable();

            // Filtering based on query options
            if (!string.IsNullOrEmpty(queryOptions.Search))
            {
                query = query.Where(e => e.Info.Contains(queryOptions.Search));
            }

            // Sorting based on query options
            if (!string.IsNullOrEmpty(queryOptions.SortField))
            {
                var propertyInfo = typeof(Expense).GetProperty(queryOptions.SortField);
                if (propertyInfo != null)
                {
                    query = queryOptions.SortDescending
                        ? query.OrderByDescending(e => EF.Property<object>(e, queryOptions.SortField))
                        : query.OrderBy(e => EF.Property<object>(e, queryOptions.SortField));
                }
            }

            // Total count for pagination
            var totalCount = await query.CountAsync();

            // Pagination
            var expenses = await query
                .Skip((paginationParameters.PageNumber - 1) * paginationParameters.PageSize)
                .Take(paginationParameters.PageSize)
                .ToListAsync();

            var expenseDtos = _mapper.Map<IEnumerable<ExpenseDTO>>(expenses);

            return new PagedResult<ExpenseDTO>
            {
                Items = expenseDtos,
                TotalCount = totalCount,
                PageNumber = paginationParameters.PageNumber,
                PageSize = paginationParameters.PageSize
            };
        }

        public async Task<ExpenseDTO> GetExpenseByIdAsync(int id)
        {
            var expense = await _context.Expenses.FindAsync(id);
            if (expense == null)
            {
                _logger.LogWarning($"Expense with ID {id} not found.");
                throw new KeyNotFoundException("Expense not found.");
            }

            return _mapper.Map<ExpenseDTO>(expense);
        }

        public async Task<ExpenseDTO> CreateExpenseAsync(ExpenseDTO expenseDto)
        {
            var expenseEntity = _mapper.Map<Expense>(expenseDto);

            await _context.Expenses.AddAsync(expenseEntity);
            await _context.SaveChangesAsync();

            return _mapper.Map<ExpenseDTO>(expenseEntity);
        }

        public async Task<ExpenseDTO> UpdateExpenseAsync(int id, ExpenseDTO expenseDto)
        {
            var existingExpense = await _context.Expenses.FindAsync(id);
            if (existingExpense == null)
            {
                _logger.LogWarning($"Expense with ID {id} not found for update.");
                throw new KeyNotFoundException("Expense not found.");
            }

            _mapper.Map(expenseDto, existingExpense); // Maps updates to the existing entity
            _context.Expenses.Update(existingExpense);
            await _context.SaveChangesAsync();

            return _mapper.Map<ExpenseDTO>(existingExpense);
        }



        public async Task<int> DeleteMultipleExpensesAsync(IEnumerable<int> ids)
        {
            var expensesToDelete = await _context.Expenses
                .Where(e => ids.Contains(e.Id))
                .ToListAsync();

            if (!expensesToDelete.Any())
            {
                _logger.LogWarning("No matching expenses found for bulk deletion.");
                return 0;
            }

            _context.Expenses.RemoveRange(expensesToDelete);
            await _context.SaveChangesAsync();

            return expensesToDelete.Count;
        }

        public async Task<decimal> CalculateTotalExpensesAsync()
        {
            var totalExpenses = await _context.Expenses
                .SumAsync(e => e.Amount);

            return totalExpenses;
        }



    }
}
