using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;

namespace BusinessWebAPI.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class EntriesProxyController : ControllerBase {

        [HttpGet]
        // GET: http://localhost:5147/api/entriesproxy
        public IActionResult Get() {
            RestClient client = new RestClient("http://localhost:5208");
            RestRequest request = new RestRequest("/api/entries", Method.Get);
            RestResponse response = client.Execute(request);
            //String value = JsonConvert.DeserializeObject<String>(response.Content);
            return Ok(response.Content);
        }
    }
}
