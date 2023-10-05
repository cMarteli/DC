using Microsoft.AspNetCore.Mvc;
using WebAPIDataServer.Models;

//Controller for the number of entries
namespace WebAPIDataServer.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class EntriesController : ControllerBase {
        [HttpGet]
        // GET: http://localhost:5208/api/entries
        public IActionResult Get() {
            return Ok(UserList.GetNumberOfEntries());
        }
    }
}
