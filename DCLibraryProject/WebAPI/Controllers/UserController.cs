using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Models.User;

namespace WebAPI.Controllers {
    public class UserController : Controller {

        //GET: api/user/all
        [HttpGet]
        public IEnumerable<User> All() {
            var dbData = UserList.AllUsers();
            return dbData;
        }

        //GET: api/user TODO: View not implemented
        [HttpGet]
        public IActionResult Index() {
            return View();
        }

        //GET: api/user/detail/{acctNo}
        [HttpGet]
        public IActionResult Detail(uint acctNo) {
            User user = UserList.GetUserByAcct(acctNo);
            if (user == null) {
                return NotFound();
            }
            else {
                return new ObjectResult(user) { StatusCode = 200 };
            }
        }

        //POST: api/user/detail (body: User details)
        [HttpPost]
        public IActionResult Detail([FromBody] User user) {
            try {
                UserList.AddUser(user);
            } catch (ArgumentException e) {
                return BadRequest(new { Message = e.Message });
            }

            var response = new { Message = "User created successfully" };
            return new ObjectResult(response) {
                StatusCode = 200,
                ContentTypes = { "application/json" }
            };
        }

        //PUT: api/user/detail/{acctNo}
        [HttpDelete]
        public IActionResult Delete(uint acctNo) {
            User user = UserList.GetUserByAcct(acctNo);
            if (user == null) {
                return NotFound();
            }
            else {
                UserList.users.Remove(user);
                var response = new { Message = "User deleted successfully" };
                return new ObjectResult(response) { StatusCode = 200 };
            }
        }
    }
}
