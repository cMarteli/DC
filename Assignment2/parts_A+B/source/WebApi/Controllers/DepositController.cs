using LocalDBWebApiUsingEF.Data;
using LocalDBWebApiUsingEF.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LocalDBWebApiUsingEF.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepositController : ControllerBase
    {
        private readonly DBManager _context;

        public DepositController(DBManager context)
        {
            _context = context;
        }

        // POST: api/deposit
        [HttpPost]
        public async Task<ActionResult<BankAccount>> ProcessDeposit(Transaction transact)
        {
            var account = await _context.BankAccounts
                                     .FirstOrDefaultAsync(b => b.AccountNumber == transact.FromAccountNumber);
            if (account == null)
            {
                return NotFound("Bank account not found.");
            }
            if (transact.Amount == 0.0)
            {
                return BadRequest("Invalid amount - Can't deposit 0.0");
            }
            account.Balance = account.Balance + transact.Amount;
            account.ToTransactions.Add(transact);
            _context.Entry(account).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.BankAccounts.Any(e => e.AccountNumber == transact.FromAccountNumber))
                {
                    return NotFound("Bank account not found. ");
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }

    }
}