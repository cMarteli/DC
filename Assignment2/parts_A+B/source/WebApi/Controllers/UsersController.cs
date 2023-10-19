using System.Text.RegularExpressions;
using LocalDBWebApiUsingEF.Data;
using LocalDBWebApiUsingEF.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace LocalDBWebApiUsingEF.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly DBManager _context;

        public UsersController(DBManager context)
        {
            _context = context;
        }

        // GET: api/Users (with BankAccounts)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.Include(u => u.BankAccounts).ToListAsync();
        }


        // POST: api/Users
        [HttpPost]
        public async Task<ActionResult<User>> CreateUser(User user)
        {
            if (!ValidUser(user))
            {
                return BadRequest("Invalid user details provided.");
            }
            try
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetUserByEmail), new { email = user.Email }, user);
            }
            catch (DbUpdateException)
            {
                return BadRequest("User already exists with this username.");
            }
        }

        // GET: api/Users/{entry} (with BankAccounts)
        [HttpGet("{entry}")]
        public async Task<ActionResult<User>> GetUserByEmail(string entry)
        {
            if (!ValidUsername(entry) && !ValidEmail(entry))
            {
                return BadRequest("Invalid user details provided.");
            }
            // If user searches via email
            if (entry.Contains("@"))
            {
                var user = await _context.Users.Include(u => u.BankAccounts).FirstOrDefaultAsync(u => u.Email == entry);
                if (user == null)
                {
                    return NotFound("User not found.");
                }
                return user;
            }
            // If user searches by username
            else
            {
                var user = await _context.Users.Include(u => u.BankAccounts).FirstOrDefaultAsync(u => u.Username == entry);
                if (user == null)
                {
                    return NotFound("User not found.");
                }
                return user;
            }
        }

        // PUT: api/Users/{username}
        [HttpPut("{username}")]
        public async Task<IActionResult> UpdateUserProfile(string username, User updatedUser)
        {
            if (!ValidUsername(username) || !ValidUser(updatedUser))
            {
                return BadRequest("Invalid user details provided.");
            }
            var existingUser = await _context.Users.FindAsync(username);
            if (existingUser == null)
            {
                return NotFound("User not found.");
            }

            // Update properties
            existingUser.Name = updatedUser.Name;
            existingUser.Email = updatedUser.Email;
            existingUser.Address = updatedUser.Address;
            existingUser.Phone = updatedUser.Phone;
            existingUser.Password = updatedUser.Password;
            existingUser.Picture = updatedUser.Picture;
            existingUser.SessionID = updatedUser.SessionID;

            _context.Entry(existingUser).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Users.Any(e => e.Username == username))
                {
                    return NotFound("User not found.");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }


        // DELETE: api/Users/{username}
        [HttpDelete("{username}")]
        public async Task<IActionResult> DeleteUser(string username)
        {
            if (!ValidUsername(username))
            {
                return BadRequest("Invalid user details provided.");
            }
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null)
            {
                return NotFound("User not found.");
            }
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private static bool ValidUser(User user)
        {
            if (!ValidUsername(user.Username))
            {
                return false;
            }
            if (user.Email != null && !ValidEmail(user.Email))
            {
                return false;
            }
            return true;
        }

        public static bool ValidUsername(string username)
        {
            string pattern = @"^[a-zA-Z0-9]+$";
            return Regex.IsMatch(username, pattern);
        }

        private static bool ValidEmail(string email)
        {
            string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            return Regex.IsMatch(email, pattern);
        }
    }

}
