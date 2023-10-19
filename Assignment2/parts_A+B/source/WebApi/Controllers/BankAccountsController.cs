using LocalDBWebApiUsingEF.Data;
using LocalDBWebApiUsingEF.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Models;

namespace LocalDBWebApiUsingEF.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BankAccountsController : ControllerBase
    {
        private readonly DBManager _context;

        public BankAccountsController(DBManager context)
        {
            _context = context;
        }

        // GET: api/BankAccounts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BankAccount>>> GetAllBankAccounts()
        {
            return await _context.BankAccounts
                    .Include(b => b.User)
                    .Include(b => b.FromTransactions)
                    .Include(b => b.ToTransactions)
                    .ToListAsync();
        }

        // POST: api/BankAccounts
        [HttpPost]
        public async Task<ActionResult<BankAccount>> CreateBankAccount(BankAccount account)
        {
            if (!UsersController.ValidUsername(account.UserUsername))
            {
                return BadRequest("Invalid bank account user details entered.");
            }
            try
            {
                _context.BankAccounts.Add(account);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetBankAccount), new { id = account.AccountNumber }, account);
            }
            catch (DbUpdateException)
            {
                return BadRequest("Bank account already exists with this bank account number.");
            }
        }

        // POST: api/BankAccounts/Transfer
        [HttpPost("Transfer")]
        public async Task<IActionResult> TransferMoney([FromBody] TransferRequest request)
        {
            // Validate From and To account numbers
            if (request.FromAccountNumber == request.ToAccountNumber)
            {
                return BadRequest("Cannot transfer to the same account.");
            }

            // Find the accounts involved in the transfer
            var fromAccount = await _context.BankAccounts.FindAsync(request.FromAccountNumber);
            var toAccount = await _context.BankAccounts.FindAsync(request.ToAccountNumber);

            if (fromAccount == null || toAccount == null)
            {
                return BadRequest("Invalid account number(s) provided.");
            }

            // Check sufficient funds
            if (fromAccount.Balance < request.Amount)
            {
                return BadRequest("Insufficient funds.");
            }

            // Perform the transfer
            fromAccount.Balance -= request.Amount;
            toAccount.Balance += request.Amount;

            // Create a new transaction record
            var transaction = new Transaction
            {
                FromAccountNumber = request.FromAccountNumber,
                ToAccountNumber = request.ToAccountNumber,
                Amount = request.Amount,
                Description = request.Description,
                Timestamp = DateTime.UtcNow
            };

            // Update the database
            _context.Transactions.Add(transaction);
            _context.Entry(fromAccount).State = EntityState.Modified;
            _context.Entry(toAccount).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Transfer successful." });
        }


        // GET: api/BankAccounts/{accountNumber}
        [HttpGet("{accountNumber}")]
        public async Task<ActionResult<BankAccount>> GetBankAccount(int accountNumber)
        {
            var account = await _context.BankAccounts
                            .Include(b => b.User)
                            .Include(b => b.FromTransactions)
                            .Include(b => b.ToTransactions)
                            .FirstOrDefaultAsync(b => b.AccountNumber == accountNumber);

            if (account == null)
            {
                return NotFound("Bank account not found.");
            }
            return account;
        }

        // PUT: api/BankAccounts/{accountNumber}
        [HttpPut("{accountNumber}")]
        public async Task<IActionResult> UpdateBankAccount(int accountNumber, BankAccount account)
        {
            if (accountNumber != account.AccountNumber)
            {
                return BadRequest("Bank account numbers do not match.");
            }
            if (!UsersController.ValidUsername(account.UserUsername))
            {
                return BadRequest("Invalid bank account user details entered.");
            }
            _context.Entry(account).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.BankAccounts.Any(e => e.AccountNumber == accountNumber))
                {
                    return NotFound("Bank account not found.");
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }

        // DELETE: api/BankAccounts/{accountNumber}
        [HttpDelete("{accountNumber}")]
        public async Task<IActionResult> DeleteBankAccount(int accountNumber)
        {
            var account = await _context.BankAccounts.FindAsync(accountNumber);
            if (account == null)
            {
                return NotFound("Bank account not found.");
            }
            _context.BankAccounts.Remove(account);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // GET: api/BankAccounts/{accountNumber}/transactions
        [HttpGet("{accountNumber}/transactions")]
        public async Task<ActionResult<IEnumerable<Transaction>>> GetTransactionsForAccount(int accountNumber)
        {
            var fromTransactions = await _context.Transactions
                                                 .Where(t => t.FromAccountNumber == accountNumber)
                                                 .ToListAsync();

            var toTransactions = await _context.Transactions
                                               .Where(t => t.ToAccountNumber == accountNumber)
                                               .ToListAsync();

            return fromTransactions.Concat(toTransactions).ToList();
        }
    }
}
