using Microsoft.AspNetCore.Mvc;

namespace web_backend.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Profile(int id)
        {
            ViewBag.UserId = id;
            return View();
        }
    }
}