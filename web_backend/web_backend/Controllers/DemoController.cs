using Microsoft.AspNetCore.Mvc;

namespace web_backend.Controllers
{
    public class DemoController : Controller
    {
        // QR kodu okutan ziyaretçiler direkt bu sayfaya düşecek
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
    }
}
