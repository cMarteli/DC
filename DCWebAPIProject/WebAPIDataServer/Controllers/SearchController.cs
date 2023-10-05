using Microsoft.AspNetCore.Mvc;
using Shared;
using WebAPIDataServer.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

// Search for a user by name
namespace WebAPIDataServer.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase {

        //Search for a user by name
        [HttpPost]
        public IActionResult Post([FromBody] SearchData sData) {
            try {
                User foundUser = UserList.GetUserByName(sData.SearchStr);

                if (foundUser != null) {
                    return Ok(foundUser); // 200 OK with user data
                }
                else {
                    return NotFound(new ErrorViewModel { 
                        RequestId = Guid.NewGuid().ToString(),
                        ErrorMessage = "User not found." 
                    }); // 404 Not Found with a message
                }
            } catch (Exception ex) {
                return StatusCode(500, new ErrorViewModel { 
                    RequestId = Guid.NewGuid().ToString(),
                    ErrorMessage = $"Internal server error: {ex.Message}" 
                }); // 500 Internal Server Error with error message
            }
        }
    }
}
