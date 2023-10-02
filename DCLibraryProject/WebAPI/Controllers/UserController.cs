using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Models.User;

namespace WebAPI.Controllers {
    public class UserController : Controller {

        [HttpGet]
        public IEnumerable<User> Details() {
            var dbData = UserList.Instance.UserData();
            return dbData;
        }

        [HttpGet]
        public IActionResult Index() {
            return View();
        }

        [HttpGet]
        public IActionResult Detail(uint acctNo) {
            User user = UserList.Instance.GetUserByAcct(acctNo);
            if (user == null) {
                return NotFound();
            }
            else {
                return new ObjectResult(user) { StatusCode = 200 };
            }
        }

        [HttpPost]
        public IActionResult Detail([FromBody] User user) {
            try {
                UserList.Instance.AddUser(user);
            } catch (ArgumentException e) {
                return BadRequest(new { Message = e.Message });
            }

            var response = new { Message = "User created successfully" };
            return new ObjectResult(response) {
                StatusCode = 200,
                ContentTypes = { "application/json" }
            };
        }
    }
}
