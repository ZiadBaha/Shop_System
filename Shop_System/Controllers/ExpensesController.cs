using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShopSystem.Core.Dtos.Program;
using ShopSystem.Core.Dtos;
using ShopSystem.Core.Enums;
using ShopSystem.Core.Models;
using ShopSystem.Core.Services.Programe;

namespace Shop_System.Controllers
{
    public class ExpensesController : ApiBaseController
    {
        private readonly IExpenseRepository _expenseService;
        private readonly ILogger<ExpensesController> _logger;

        public ExpensesController(IExpenseRepository expenseService, ILogger<ExpensesController> logger)
        {
            _expenseService = expenseService;
            _logger = logger;
        }

<<<<<<< HEAD
=======
        // GET: api/expenses
>>>>>>> c494a203df9e1d58e56da61dbd02ac6c77ab300c
        [HttpGet]
        public async Task<IActionResult> GetAllExpenses([FromQuery] PaginationParameters paginationParameters, [FromQuery] QueryOptions queryOptions)
        {
            var result = await _expenseService.GetAllExpensesAsync(paginationParameters, queryOptions);
            return Ok(new ContentContainer<PagedResult<ExpenseDTO>>(result, "Expenses retrieved successfully."));
        }

<<<<<<< HEAD
=======
        // GET: api/expenses/{id}
>>>>>>> c494a203df9e1d58e56da61dbd02ac6c77ab300c
        [HttpGet("{id}")]
        public async Task<IActionResult> GetExpenseById(int id)
        {
            try
            {
                var expense = await _expenseService.GetExpenseByIdAsync(id);
                return Ok(new ContentContainer<ExpenseDTO>(expense, "Expense retrieved successfully."));
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex.Message);
                return NotFound(new ContentContainer<string>(null, ex.Message));
            }
        }

        // POST: api/expenses
        [HttpPost]
        public async Task<IActionResult> CreateExpense([FromBody] ExpenseDTO expenseDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ContentContainer<string>(null, "Invalid expense data."));

            var createdExpense = await _expenseService.CreateExpenseAsync(expenseDto);
            return CreatedAtAction(nameof(GetExpenseById), new { id = createdExpense.ID }, new ContentContainer<ExpenseDTO>(createdExpense, "Expense created successfully."));
        }

        // PUT: api/expenses/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateExpense(int id, [FromBody] ExpenseDTO expenseDto)
        {
            if (!ModelState.IsValid)
            {
                var errorMessages = string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return BadRequest(new ContentContainer<string>(null, errorMessages));
            }

            try
            {
                var updatedExpense = await _expenseService.UpdateExpenseAsync(id, expenseDto);
                return Ok(new ContentContainer<ExpenseDTO>(updatedExpense, "Expense updated successfully."));
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex.Message);
                return NotFound(new ContentContainer<string>(null, ex.Message));
            }
        }



        // DELETE: api/expenses/delete-multiple
        [HttpDelete("delete-multiple")]
        public async Task<IActionResult> DeleteMultipleExpenses([FromForm] IEnumerable<int> ids)
        {
            var deletedCount = await _expenseService.DeleteMultipleExpensesAsync(ids);

            if (deletedCount == 0)
                return NotFound(new ContentContainer<string>(null, "No matching expenses found to delete."));

            return Ok(new ContentContainer<int>(deletedCount, $"{deletedCount} expenses deleted successfully."));
        }

        // GET: api/expenses/total
        [HttpGet("total")]
        public async Task<IActionResult> CalculateTotalExpenses()
        {
            var totalExpenses = await _expenseService.CalculateTotalExpensesAsync();
            return Ok(new ContentContainer<decimal>(totalExpenses, $"Total expenses calculated successfully."));
        }
    }

}
