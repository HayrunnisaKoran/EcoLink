using Microsoft.AspNetCore.Mvc;

namespace web_backend.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            ViewBag.FacultyName = "Hasan Ferdi Turgutlu Teknoloji Fakültesi";
            ViewBag.TotalWaste = "2.450 kg";
            ViewBag.ActiveUsers = "1.842";
            ViewBag.TotalPoints = "125.600";
            ViewBag.ActiveClubs = "23";

            return View();
        }
    }
}