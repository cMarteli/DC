using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.MSIdentity.Shared;
using Newtonsoft.Json;
using WebApp.Models;

namespace WebApp.Controllers
{
    [Route("api/[controller]")]
    public class AdminController : Controller
    {
        [HttpGet("view")]
        public async Task<IActionResult> GetViewAsync()
        {
            if (Request.Cookies.ContainsKey("SessionID"))
            {
                if (AdminLoginController.verifyAdminSessionID(Request.Cookies["Username"], Request.Cookies["SessionID"]))
                {
                    var client = new HttpClient();
                    client.BaseAddress = new Uri("http://localhost:5181/");
                    var admin = await client.GetFromJsonAsync<Admin>("api/admins/" + Request.Cookies["Username"]);
                    ViewData["Admin"] = admin;
                    return PartialView("AdminViewAuthenticated");
                }

            }
            return PartialView("AdminViewDefault");
        }

        [HttpPost("contact")]
        public async Task<IActionResult> UpdateAdminContact([FromBody] Admin adminUpdate)
        {
            if (Request.Cookies.ContainsKey("SessionID"))
            {
                if (AdminLoginController.verifyAdminSessionID(Request.Cookies["Username"], Request.Cookies["SessionID"]))
                {
                    var client = new HttpClient();
                    client.BaseAddress = new Uri("http://localhost:5181/");
                    var admin = await client.GetFromJsonAsync<Admin>("api/admins/" + Request.Cookies["Username"]);

                    if (admin != null)
                    {
                        if (adminUpdate.Phone != null && adminUpdate.Phone != "")
                        {
                            admin.Phone = adminUpdate.Phone;
                        }
                        if (adminUpdate.Email != null && adminUpdate.Email != "")
                        {
                            admin.Email = adminUpdate.Email;
                        }
                        var updateTask = client.PutAsJsonAsync<Admin>("api/admins/" + admin.Username, admin);
                        updateTask.Wait();
                        ViewData["Admin"] = admin;
                        return PartialView("AdminViewAuthenticated");
                    }
                }
            }
            return PartialView("AdminViewDefault");
        }

        [HttpPost("password")]
        public IActionResult UpdateAdminPassword([FromBody] Admin adminUpdate)
        {
            if (Request.Cookies.ContainsKey("SessionID"))
            {
                if (AdminLoginController.verifyAdminSessionID(Request.Cookies["Username"], Request.Cookies["SessionID"]))
                {
                    var client = new HttpClient();
                    client.BaseAddress = new Uri("http://localhost:5181/");
                    var task = client.GetFromJsonAsync<Admin>("api/admins/" + Request.Cookies["Username"]);
                    task.Wait();
                    var admin = task.Result;

                    if (admin != null)
                    {
                        if (adminUpdate.Password != null)
                        {
                            admin.Password = adminUpdate.Password;
                        }
                        var updateTask = client.PutAsJsonAsync<Admin>("api/admins/" + admin.Username, admin);
                        updateTask.Wait();
                        ViewData["Admin"] = admin;
                        return PartialView("AdminViewAuthenticated");
                    }
                }
            }
            return PartialView("AdminViewDefault");
        }

        [HttpGet("users")]
        public IActionResult GetAdminUsersView()
        {
            if (Request.Cookies.ContainsKey("SessionID"))
            {
                if (AdminLoginController.verifyAdminSessionID(Request.Cookies["Username"], Request.Cookies["SessionID"]))
                {
                    return PartialView("Users/AdminUsersViewAuthenticated");
                }
            }
            return PartialView("Users/AdminUsersViewDefault");
        }

        [HttpPost("users/create")]
        public IActionResult AdminCreateUser([FromBody] User newUser)
        {
            var response = new { success = false };
            if (Request.Cookies.ContainsKey("SessionID"))
            {
                if (AdminLoginController.verifyAdminSessionID(Request.Cookies["Username"], Request.Cookies["SessionID"]))
                {
                    User user = new User(newUser.Username)
                    {
                        Name = newUser.Name,
                        Email = newUser.Email,
                        Address = newUser.Address,
                        Phone = newUser.Phone,
                        Picture = newUser.Picture,
                        Password = newUser.Password
                    };

                    var client = new HttpClient();
                    client.BaseAddress = new Uri("http://localhost:5181/");
                    var task = client.PostAsJsonAsync<User>("api/users/", user);
                    task.Wait();
                    var result = task.Result;
                    if (result.StatusCode == HttpStatusCode.OK || result.StatusCode == HttpStatusCode.Created)
                    {
                        response = new { success = true };
                    }
                }
            }
            return Json(response);
        }

        [HttpPut("users/edit/{username}")]
        public IActionResult AdminEditUser([FromRoute] string username, [FromBody] User editUser)
        {
            var response = new { success = false };
            if (Request.Cookies.ContainsKey("SessionID"))
            {
                if (AdminLoginController.verifyAdminSessionID(Request.Cookies["Username"], Request.Cookies["SessionID"]))
                {
                    var client = new HttpClient();
                    client.BaseAddress = new Uri("http://localhost:5181/");
                    var task = client.GetFromJsonAsync<User>("api/users/" + username);
                    try
                    {
                        task.Wait();
                    }
                    catch (AggregateException) // HttpRequestException
                    {
                        response = new
                        {
                            success = false
                        };
                        return Json(response);
                    }
                    var verifyUser = task.Result;
                    if (verifyUser != null && verifyUser.Username == username && editUser.Username == username)
                    {
                        verifyUser.Name = verifyUser.Name == editUser.Name ? verifyUser.Name : editUser.Name;
                        verifyUser.Email = verifyUser.Email == editUser.Email ? verifyUser.Email : editUser.Email; ;
                        verifyUser.Address = verifyUser.Address == editUser.Address ? verifyUser.Address : editUser.Address; ;
                        verifyUser.Phone = verifyUser.Phone == editUser.Phone ? verifyUser.Phone : editUser.Phone; ;
                        verifyUser.Picture = verifyUser.Picture == editUser.Picture ? verifyUser.Picture : editUser.Picture; ;

                        var updateTask = client.PutAsJsonAsync<User>("api/users/" + verifyUser.Username, verifyUser);
                        updateTask.Wait();
                        var result = updateTask.Result;
                        if (result.StatusCode == HttpStatusCode.OK || result.StatusCode == HttpStatusCode.Created || result.StatusCode == HttpStatusCode.NoContent)
                        {
                            response = new { success = true };
                        }
                    }
                }
            }
            return Json(response);
        }

        [HttpDelete("users/delete/{username}")]
        public IActionResult AdminDeleteUser([FromRoute] string username)
        {
            var response = new { success = false };
            if (Request.Cookies.ContainsKey("SessionID"))
            {
                if (AdminLoginController.verifyAdminSessionID(Request.Cookies["Username"], Request.Cookies["SessionID"]))
                {
                    var client = new HttpClient();
                    client.BaseAddress = new Uri("http://localhost:5181/");
                    var task = client.DeleteAsync("api/users/" + username);
                    task.Wait();
                    var result = task.Result;
                    if (result.StatusCode == HttpStatusCode.OK || result.StatusCode == HttpStatusCode.Created || result.StatusCode == HttpStatusCode.NoContent)
                    {
                        response = new { success = true };
                    }
                }
            }
            return Json(response);
        }

        [HttpGet("users/view/{searchterm}")]
        public IActionResult AdminSearchUser(string searchterm)
        {
            if (Request.Cookies.ContainsKey("SessionID"))
            {
                if (AdminLoginController.verifyAdminSessionID(Request.Cookies["Username"], Request.Cookies["SessionID"]))
                {
                    var client = new HttpClient();
                    client.BaseAddress = new Uri("http://localhost:5181/");
                    var task = client.GetFromJsonAsync<User>("api/users/" + searchterm);
                    try
                    {
                        task.Wait();
                        var user = task.Result;

                        if (user != null)
                        {
                            ViewData["User"] = user;
                            return PartialView("Users/AdminUsersViewAuthenticated");
                        }
                    }
                    catch (AggregateException ex)
                    {
                        Console.WriteLine("Unable to find user: " + ex.Message);
                        return PartialView("Users/AdminUsersErrorView");
                    }
                }
            }
            return PartialView("Users/AdminUsersViewDefault");
        }

        [HttpGet("transactions")]
        public IActionResult GetAdminTransactionsView()
        {
            if (Request.Cookies.ContainsKey("SessionID"))
            {
                if (AdminLoginController.verifyAdminSessionID(Request.Cookies["Username"], Request.Cookies["SessionID"]))
                {
                    var client = new HttpClient();
                    client.BaseAddress = new Uri("http://localhost:5181/");
                    var task = client.GetFromJsonAsync<List<BankAccount>>("api/bankaccounts");
                    task.Wait();
                    var bankAccounts = task.Result;

                    ViewData["BankAccounts"] = bankAccounts;
                    return PartialView("Transactions/AdminTransactionsViewAuthenticated");
                }
            }
            return PartialView("Transactions/AdminTransactionsViewDefault");
        }
    }
}