using Microsoft.AspNetCore.Mvc;

namespace web_backend.Controllers
{
    public class AuthController : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            if (email == "admin@cbu.edu.tr" && password == "123456")
            {
                return RedirectToAction("Index", "Dashboard");
            }

            ViewBag.Error = "E-posta veya şifre hatalı.";
            return View();
        }

        [HttpPost]
        public IActionResult Register()
        {
            return View();
        }
        public IActionResult Logout()
        {
            return RedirectToAction("Login", "Auth");
        }
    }
}