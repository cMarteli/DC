using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI {
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase {

        // GET: api/values/add/{num1}/{num2}
        [HttpGet]
        [Route("add/{num1}/{num2}")]
        public IActionResult Add(int num1, int num2) {
            int result = num1 + num2;
            var response = new { Message = result };
            return Ok(response);
        }

        //Using query string
        //GET: api/values/add?num1=1&num2=2
        [Route("add")]
        [HttpGet]
        public IActionResult AddFromQuery([FromQuery] int num1, [FromQuery] int num2) {
            int result = num1 + num2;
            var response = new { Message = result };
            return Ok(response);
        }
        
    }
}
