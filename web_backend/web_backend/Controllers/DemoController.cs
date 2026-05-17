using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using web_backend.Models;

namespace web_backend.Controllers
{
    public class DemoController : Controller
    {
        public IActionResult Index()
        {
            // QR Kod buraya çarpar çarpmaz, kullanıcıyı hissettirmeden 
            // MobileController'ın Index sayfasına fırlatıyoruz!
            return RedirectToAction("Index", "Mobile");
        }
    }
}
