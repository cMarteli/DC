using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;
using Shared;
using Shared.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BusinessWebAPI.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class SearchProxyController : ControllerBase {

        [HttpPost]
        // POST: http://localhost:5147/api/searchproxy/
        // Body: { "searchStr": "John" }
        public IActionResult Post([FromBody] SearchData sData) {
            try {
                RestClient client = new RestClient("http://localhost:5208");
                RestRequest request = new RestRequest("/api/Search", Method.Post);
                request.AddJsonBody(sData);

                RestResponse response = client.Execute(request);

                if (response.StatusCode == System.Net.HttpStatusCode.OK) {
                    User user = JsonConvert.DeserializeObject<User>(response.Content);
                    return Ok(user);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound) {
                    ErrorViewModel errorResponse = JsonConvert.DeserializeObject<ErrorViewModel>(response.Content);
                    return NotFound(errorResponse);
                }
                else {
                    ErrorViewModel errorResponse = JsonConvert.DeserializeObject<ErrorViewModel>(response.Content);
                    return StatusCode((int)response.StatusCode, errorResponse);
                }
            } catch (Exception ex) {
                return StatusCode(500, new { message = $"Proxy error: {ex.Message}" }); // 500 Internal Server Error with proxy error message
            }
        }

    }
}
