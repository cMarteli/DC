using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks.Dataflow;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;


namespace WebApp.Controllers
{
    [Route("api/[controller]")]
    public class AdminLoginController : Controller
    {
        private readonly ILogger<AdminLoginController> _logger;

        public AdminLoginController(ILogger<AdminLoginController> logger)
        {
            _logger = logger;
        }

        [HttpGet("defaultview")]
        public IActionResult GetDefaultView()
        {
            if (Request.Cookies.ContainsKey("SessionID"))
            {
                if (verifyAdminSessionID(Request.Cookies["Username"], Request.Cookies["SessionID"]))
                {
                    return PartialView("AdminLoginAuthenticatedView");
                }
            }
            // Return the partial view as HTML
            return PartialView("AdminLoginDefaultView");
        }


        [HttpGet("authview")]
        public IActionResult GetAdminLoginAuthenticatedView()
        {
            if (Request.Cookies.ContainsKey("SessionID"))
            {
                if (verifyAdminSessionID(Request.Cookies["Username"], Request.Cookies["SessionID"]))
                {
                    return PartialView("AdminLoginAuthenticatedView");
                }
            }
            // Return the partial view as HTML
            return PartialView("AdminLoginErrorView");
        }

        [HttpGet("error")]
        public IActionResult GetLoginErrorView()
        {
            return PartialView("AdminLoginErrorView");
        }

        [HttpPost("auth")]
        public IActionResult Authenticate([FromBody] Admin admin)
        {
            // Return the partial view as HTML
            var response = new { login = false };
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:5181/");
                var task = client.GetFromJsonAsync<Admin>("api/admins/" + admin.Username);
                task.Wait();
                var verifyUser = task.Result;
                if (verifyUser != null && admin.Username == verifyUser.Username && admin.Password == verifyUser.Password)
                {
                    string sessionID = generateSessionID();
                    verifyUser.SessionID = sessionID;
                    Response.Cookies.Append("Username", admin.Username);
                    Response.Cookies.Append("SessionID", sessionID);
                    var updateTask = client.PutAsJsonAsync<Admin>("api/admins/" + admin.Username, verifyUser);
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

        public static bool verifyAdminSessionID(string? username, string? sessionID)
        {
            if (username == null || sessionID == null)
            {
                return false;
            }
            using (var client = new HttpClient())
            {
                try
                {
                    client.BaseAddress = new Uri("http://localhost:5181/");
                    var task = client.GetFromJsonAsync<Admin>("api/admins/" + username);
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