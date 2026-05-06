using Microsoft.AspNetCore.Mvc;
using web_backend.Models;
using Microsoft.EntityFrameworkCore;
using web_backend;

namespace web_backend.Controllers
{

    public class DashboardController : Controller
    {
        private readonly AppDbContext _context;
        // Veritabaný baðlantýsýný buraya enjekte ediyoruz
        public DashboardController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            // Veritabanýndaki gerçek toplamlarý çekiyoruz
            ViewBag.TotalWaste = await _context.WasteRecords.SumAsync(r => r.Amount) + " kg";
            ViewBag.ActiveUsers = await _context.Users.CountAsync(u => u.IsActive);
            ViewBag.TotalPoints = await _context.Users.SumAsync(u => u.TotalPoints);

            // Son kayýtlarý da tabloya göndermek için
            var recentRecords = await _context.WasteRecords
                .Include(r => r.WasteType)
                .Include(r => r.User)
                .OrderByDescending(r => r.CreatedAt)
                .Take(5)
                .ToListAsync();

            return View(recentRecords);
        }
    }
}