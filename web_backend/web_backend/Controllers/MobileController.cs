using Microsoft.AspNetCore.Mvc;

namespace web_backend.Controllers
{
    public class MobileController : Controller
    {
        public IActionResult Index()
        {
            ViewData["Title"] = "EcoLink Mobil";
            return View();
        }

        public IActionResult Scan(string? binCode)
        {
            ViewData["Title"] = "Atık Doğrulama";

            ViewBag.BinCode = string.IsNullOrWhiteSpace(binCode)
                ? "KUTU-1023"
                : binCode;

            return View();
        }

        [HttpPost]
        public IActionResult SubmitWaste(string binCode, string wasteType, decimal amount)
        {
            TempData["BinCode"] = string.IsNullOrWhiteSpace(binCode) ? "KUTU-1023" : binCode;
            TempData["WasteType"] = string.IsNullOrWhiteSpace(wasteType) ? "Plastik" : wasteType;
            TempData["Amount"] = amount <= 0 ? "1" : amount.ToString("0.##");
            TempData["EarnedPoint"] = wasteType == "Organik" ? "40" : "25";

            return RedirectToAction(nameof(Success));
        }

        public IActionResult Success()
        {
            ViewData["Title"] = "Doğrulama Başarılı";
            return View();
        }

        public IActionResult Leaderboard()
        {
            ViewData["Title"] = "Yeşil Lig";
            return View();
        }

        public IActionResult Rewards()
        {
            ViewData["Title"] = "Ödüller";
            return View();
        }

        public IActionResult Profile()
        {
            ViewData["Title"] = "Profil";
            return View();
        }
    }
}