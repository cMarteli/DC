using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;

namespace WebApp.Controllers
{
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        [HttpGet("view")]
        public async Task<IActionResult> GetViewAsync()
        {
            if (Request.Cookies.ContainsKey("SessionID"))
            {
                var cookieValue = Request.Cookies["SessionID"];
                var username = Request.Cookies["Username"];
                if (LoginController.verifySessionID(username, cookieValue))
                {
                    var client = new HttpClient
                    {
                        BaseAddress = new Uri("http://localhost:5181/")
                    };
                    var user = await client.GetFromJsonAsync<User>("api/users/" + Request.Cookies["Username"]);
                    ViewData["Users"] = user;
                    return PartialView("AccountViewAuthenticated");
                }
            }
            return PartialView("AccountViewDefault");
        }
    }
}

