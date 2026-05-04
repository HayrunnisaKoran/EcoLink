using Microsoft.AspNetCore.Mvc;

namespace web_backend.Controllers
{
    public class WasteRecordController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(string wasteType, double amount, string location)
        {
            TempData["Message"] = "Atık kaydı başarıyla oluşturuldu.";
            return RedirectToAction("Index");
        }

        public IActionResult Details(int id)
        {
            ViewBag.RecordId = id;
            return View();
        }
    }
}