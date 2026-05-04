using Microsoft.AspNetCore.Mvc;

namespace web_backend.Controllers
{
    public class RewardController : Controller
    {
        public IActionResult Badges()
        {
            return View();
        }

        public IActionResult Points()
        {
            return View();
        }

        public IActionResult Tasks()
        {
            return View();
        }
    }
}