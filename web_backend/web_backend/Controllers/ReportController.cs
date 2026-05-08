using Microsoft.AspNetCore.Mvc;

namespace web_backend.Controllers
{
    public class ReportController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult WasteStatistics()
        {
            return View();
        }

        public IActionResult CompostStatistics()
        {
            return View();
        }
    }
}