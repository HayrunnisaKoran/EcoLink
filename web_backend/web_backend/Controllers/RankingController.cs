using Microsoft.AspNetCore.Mvc;

namespace web_backend.Controllers
{
    public class RankingController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Faculties()
        {
            return View();
        }

        public IActionResult Departments()
        {
            return View();
        }
    }
}