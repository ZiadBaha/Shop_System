using ShopSystem.Core.Dtos.Program;
using ShopSystem.Core.Dtos;
using ShopSystem.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopSystem.Core.Services.Programe
{
    public interface IExpenseRepository
    {
        Task<PagedResult<ExpenseDTO>> GetAllExpensesAsync(PaginationParameters paginationParameters, QueryOptions queryOptions);
        Task<ExpenseDTO> GetExpenseByIdAsync(int id);
        Task<ExpenseDTO> CreateExpenseAsync(ExpenseDTO expenseDto);
        Task<ExpenseDTO> UpdateExpenseAsync(int id, ExpenseDTO expenseDto);
        Task<int> DeleteMultipleExpensesAsync(IEnumerable<int> ids);
        Task<decimal> CalculateTotalExpensesAsync();
    }
}
