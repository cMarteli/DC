using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using Shared;

namespace BusinessWebAPI.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class UserProxyController : ControllerBase {

        private readonly string backendBaseUrl = "http://localhost:5208";
        // Find a user by index
        [HttpGet("{id}")]
        // GET: http://localhost:5147/api/userproxy/id
        public IActionResult Details(int id) {
            RestClient client = new RestClient(backendBaseUrl);
            RestRequest request = new RestRequest($"/api/User/{id}", Method.Get);
            RestResponse response = client.Execute(request);

            if (!response.IsSuccessful) {
                return StatusCode((int)response.StatusCode, response.ErrorMessage);
            }

            User user = JsonConvert.DeserializeObject<User>(response.Content);
            return Ok(user);
        }
        // Get all users
        [HttpGet]
        // GET: http://localhost:5147/api/userproxy
        public IActionResult GetAll() {
            RestClient client = new RestClient(backendBaseUrl);
            RestRequest request = new RestRequest("/api/User", Method.Get);
            RestResponse response = client.Execute(request);

            if (!response.IsSuccessful) {
                return StatusCode((int)response.StatusCode, response.ErrorMessage);
            }

            List<User> users = JsonConvert.DeserializeObject<List<User>>(response.Content);
            return Ok(users);
        }
    }
}
