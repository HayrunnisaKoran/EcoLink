using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using web_backend.Services;
using web_backend.Models;

namespace web_backend.Controllers
{
    public class UserController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IPointTransactionService _pointTransactionService;

        public UserController(AppDbContext context, IPointTransactionService pointTransactionService)
        {
            _context = context;
            _pointTransactionService = pointTransactionService;
        }

        // Tüm kullanýcýlarý listelemek için (Yönetici Paneli için gerekebilir)
        public async Task<IActionResult> Index()
        {
            var users = await _context.Users.ToListAsync();
            return View(users);
        }

        // Kullanýcý Profil Sayfasý
        public async Task<IActionResult> Profile(int id)
        {
            // 1. Kullanýcýyý, rozetlerini ve rozet detaylarýný tek sorguda getir
            var user = await _context.Users
                .Include(u => u.UserBadges)
                    .ThenInclude(ub => ub.Badge)
                .FirstOrDefaultAsync(u => u.UserId == id);

            if (user == null) return NotFound();

            // 2. Puan dökümünü daha önce yazdýðýmýz servisten çek
            var history = await _pointTransactionService.GetUserHistoryAsync(id);

            // 3. Verileri View'a gönder
            ViewBag.PointsHistory = history;
            return View(user);
        }
    }
}