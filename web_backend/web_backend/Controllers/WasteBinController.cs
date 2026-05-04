using Microsoft.AspNetCore.Mvc;

namespace web_backend.Controllers
{
    public class WasteBinController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details(int id)
        {
            ViewBag.BinId = id;
            return View();
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(string binCode, string wasteType, string location)
        {
            TempData["Message"] = "Atık kutusu başarıyla eklendi.";
            return RedirectToAction("Index");
        }
    }
}