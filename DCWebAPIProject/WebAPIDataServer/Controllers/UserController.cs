using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAPIDataServer.Models;

// Controller for the User model can return a single user by index or all users
namespace WebAPIDataServer.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase {
        // Find a user by index
        [HttpGet("{id}")]
        // GET: http://localhost:5208/api/user/id
        public IActionResult Details(int id) {
            try {
                var user = UserList.GetUserByIndex(id);
                if (user == null) {
                    return NotFound($"User with ID {id} not found.");
                }
                return Ok(user);
            } catch (ArgumentOutOfRangeException) {
                return BadRequest($"Invalid ID: {id}");
            } catch (Exception ex) {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        // Get all users
        [HttpGet]
        // GET: http://localhost:5208/api/user
        public IActionResult GetAll() {
            return Ok(UserList.AllUsers());
        }
    }
}
