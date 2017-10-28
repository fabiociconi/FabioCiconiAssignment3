using FabioCiconiAssignment3.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FabioCiconiAssignment3.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(LoginViewModel lr, string user, string password)
        {
            lr.User = user;
            lr.Password = password;
            if (lr.IsValid())
            {
                var logado = JsonConvert.SerializeObject(user);
                HttpContext.Session.SetString("logadoUser", logado);

                return RedirectToAction("Index", "Home");
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
    }
}