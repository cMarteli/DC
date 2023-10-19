using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers {

    public class ClientController : Controller {
        private readonly ClientContext _context;

        public ClientController(ClientContext context) {
            _context = context;
        }

        // GET: client/getClients
        [HttpGet]
        public ActionResult GetClients() {
            List<Client> clientList = _context.Clients.ToList();
            string jsonResponse = JsonConvert.SerializeObject(clientList);
            return Content(jsonResponse, "application/json");
        }


        // POST: client/register 
        /* BODY: 
         * {
         * "Id": 7,
         * "IPAddress": "192.168.1.19",
         * "Port": 8098
         * } 
         */
        [HttpPost]
        public ActionResult Register([FromBody] Client client) {
            // Add client to database
            _context.Clients.Add(client);
            _context.SaveChanges();

            string jsonResponse = JsonConvert.SerializeObject(new { success = true });
            return Content(jsonResponse, "application/json");
        }

    }
}
