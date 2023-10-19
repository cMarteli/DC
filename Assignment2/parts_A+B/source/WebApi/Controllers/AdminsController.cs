using System.Text.RegularExpressions;
using LocalDBWebApiUsingEF.Data;
using LocalDBWebApiUsingEF.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace LocalDBWebApiUsingEF.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminsController : ControllerBase
    {
        private readonly DBManager _context;

        public AdminsController(DBManager context)
        {
            _context = context;
        }

        // GET: api/admins
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Admin>>> GetAdmins()
        {
            return await _context.Admins.ToListAsync();
        }

        // POST: api/Admin
        [HttpPost]
        public async Task<ActionResult<Admin>> CreateAdmin(Admin user)
        {
            if (!ValidUser(user))
            {
                return BadRequest("Invalid user details provided.");
            }
            try
            {
                _context.Admins.Add(user);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetAdminByEmail), new { email = user.Email }, user);
            }
            catch (DbUpdateException)
            {
                return BadRequest("User already exists with this username.");
            }
        }

        // GET: api/Admin/{entry}
        [HttpGet("{entry}")]
        public async Task<ActionResult<Admin>> GetAdminByEmail(string entry)
        {
            if (!ValidUsername(entry) && !ValidEmail(entry))
            {
                return BadRequest("Invalid user details provided.");
            }
            // If user searches via email
            if (entry.Contains("@"))
            {
                var user = await _context.Admins.FirstOrDefaultAsync(u => u.Email == entry);
                if (user == null)
                {
                    return NotFound("User not found.");
                }
                return user;
            }
            // If user searches by username
            else
            {
                var user = await _context.Admins.FirstOrDefaultAsync(u => u.Username == entry);
                if (user == null)
                {
                    return NotFound("User not found.");
                }
                return user;
            }
        }

        // PUT: api/Admin/{username}
        [HttpPut("{username}")]
        public async Task<IActionResult> UpdateAdminProfile(string username, Admin updatedUser)
        {
            if (!ValidUsername(username) || !ValidUser(updatedUser))
            {
                return BadRequest("Invalid user details provided.");
            }
            var existingUser = await _context.Admins.FindAsync(username);
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
                if (!_context.Admins.Any(e => e.Username == username))
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


        // DELETE: api/Admin/{username}
        [HttpDelete("{username}")]
        public async Task<IActionResult> DeleteAdmin(string username)
        {
            if (!ValidUsername(username))
            {
                return BadRequest("Invalid user details provided.");
            }
            var user = await _context.Admins.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null)
            {
                return NotFound("User not found.");
            }
            _context.Admins.Remove(user);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private static bool ValidUser(Admin user)
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
