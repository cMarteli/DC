using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks.Dataflow;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;

namespace WebApp.Controllers
{
    [Route("api/[controller]")]
    public class LoginController : Controller
    {
        private readonly ILogger<LoginController> _logger;

        public LoginController(ILogger<LoginController> logger)
        {
            _logger = logger;
        }

        [HttpGet("defaultview")]
        public IActionResult GetDefaultView()
        {
            if (Request.Cookies.ContainsKey("SessionID"))
            {
                if (verifySessionID(Request.Cookies["Username"], Request.Cookies["SessionID"]))
                {
                    return PartialView("LoginAuthenticatedView");
                }
            }
            // Return the partial view as HTML
            return PartialView("LoginDefaultView");
        }


        [HttpGet("authview")]
        public IActionResult GetLoginAuthenticatedView()
        {
            if (Request.Cookies.ContainsKey("SessionID"))
            {
                if (verifySessionID(Request.Cookies["Username"], Request.Cookies["SessionID"]))
                {
                    return PartialView("LoginAuthenticatedView");
                }
            }
            // Return the partial view as HTML
            return PartialView("LoginErrorView");
        }

        [HttpGet("error")]
        public IActionResult GetLoginErrorView()
        {
            return PartialView("LoginErrorView");
        }

        [HttpPost("auth")]
        public IActionResult Authenticate([FromBody] User user)
        {
            // Return the partial view as HTML
            var response = new { login = false };
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:5181/");
                var task = client.GetFromJsonAsync<User>("api/users/" + user.Username);
                task.Wait();
                var verifyUser = task.Result;
                if (verifyUser != null && user.Username == verifyUser.Username && user.Password == verifyUser.Password)
                {
                    string sessionID = generateSessionID();
                    verifyUser.SessionID = sessionID;
                    Response.Cookies.Append("Username", user.Username);
                    Response.Cookies.Append("SessionID", sessionID);
                    var updateTask = client.PutAsJsonAsync<User>("api/users/" + user.Username, verifyUser);
                    updateTask.Wait();
                    response = new { login = true };
                }
            }
            return Json(response);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public static bool verifySessionID(string? username, string? sessionID)
        {
            if (username == null || sessionID == null)
            {
                return false;
            }
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:5181/");
                try
                {
                    var task = client.GetFromJsonAsync<User>("api/users/" + username);
                    task.Wait();
                    var verifyUser = task.Result;
                    if (verifyUser != null && verifyUser.SessionID == sessionID)
                    {
                        return true;
                    }
                }
                catch (AggregateException)
                {
                    return false;
                }
            }
            return false;
        }

        private string generateSessionID()
        {
            const string charset = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            const int length = 32;
            string sessionID = "";
            Random random = new Random();

            for (int i = 0; i < length; i++)
            {
                int index = random.Next(charset.Length);
                sessionID += charset[index];
            }

            return sessionID;
        }
    }
}