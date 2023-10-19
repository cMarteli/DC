using DataServer.Data;
using DataServer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace DataServer.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController : Controller {
        private readonly ClientContext _context;

        public ClientController(ClientContext context) {
            _context = context;
        }

        // GET: api/client/all
        [HttpGet("all")]
        public ActionResult GetClients() {
            try {
                List<Client> clientList = _context.Clients.ToList();
                string jsonResponse = JsonConvert.SerializeObject(clientList);
                return Content(jsonResponse, "application/json");
            } catch (Exception ex) {
                return StatusCode(500, new { message = "An error occurred while fetching clients.", details = ex.Message });
            }
        }

        // GET: api/client/{ip}/{port}
        [HttpGet("{ip}/{port}")]
        public ActionResult GetClient(string ip, int port) {
            try {
                Client client = _context.Clients.FirstOrDefault(c => c.IPAddress == ip && c.Port == port);
                if (client == null) {
                    return NotFound(new { message = $"Client with IP {ip} and Port {port} not found." });
                }
                string jsonResponse = JsonConvert.SerializeObject(client);
                return Content(jsonResponse, "application/json");
            } catch (System.Exception ex) {
                return StatusCode(500, new { message = "An error occurred while fetching the client.", details = ex.Message });
            }
        }

        // DELETE: api/client/{ip}/{port}
        [HttpDelete("{ip}/{port}")]
        public async Task<IActionResult> DeleteClient(string ip, int port) {
            try {
                var client = await _context.Clients.FirstOrDefaultAsync(c => c.IPAddress == ip && c.Port == port);
                if (client == null) {
                    return NotFound(new { message = $"Client with IP {ip} and Port {port} not found." });
                }
                _context.Clients.Remove(client);
                await _context.SaveChangesAsync();
                return Ok(new { message = $"Client with IP {ip} and Port {port} removed successfully." });
            } catch (DbUpdateException ex) {
                return StatusCode(400, new { message = "Database update failed.", details = ex.Message });
            } catch (System.Exception ex) {
                return StatusCode(500, new { message = "An error occurred while removing the client.", details = ex.Message });
            }
        }



        // POST: api/client/new 
        /* BODY: 
         * {
         * "IPAddress": "192.168.1.19",
         * "Port": 8098
         * } 
         */
        [HttpPost("new")]
        public IActionResult Register([FromBody] Client client) {
            try {
                // Add client to database
                _context.Clients.Add(client);
                _context.SaveChanges();
                string jsonResponse = JsonConvert.SerializeObject(new { success = true });
                return Content(jsonResponse, "application/json");
            } catch (DbUpdateException ex) {
                // Handle database-related errors
                return StatusCode(400, new { message = "Database update failed.", details = ex.Message });
            } catch (Exception ex) {
                // Log the exception (consider using a logging framework)
                return StatusCode(500, new { message = "An error occurred while registering the client.", details = ex.Message });
            }
        }

    }
}
