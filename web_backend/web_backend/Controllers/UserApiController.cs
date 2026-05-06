using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using web_backend.Models;
using web_backend.Services;

namespace web_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserApiController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IPointTransactionService _transactionService;

        public UserApiController(AppDbContext context, IPointTransactionService transactionService)
        {
            _context = context;
            _transactionService = transactionService;
        }

        // 1. PROFİL EKRANI VERİSİ (Görseldeki Hayrünnisa ekranı için)
        [HttpGet("profile/{userId}")]
        public async Task<IActionResult> GetProfileData(int userId)
        {
            var user = await _context.Users
                .Include(u => u.UserBadges).ThenInclude(ub => ub.Badge)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null) return NotFound();

            // Yeşil Matematik Formülleri (Temsili)
            // Örn: Her 1 kg atık = 2.5 kg CO2 tasarrufu, Her 50 kg = 1 Fidan
            var totalWasteWeight = await _context.WasteRecords
                .Where(r => r.UserId == userId)
                .SumAsync(r => (double?)r.Amount) ?? 0;

            return Ok(new
            {
                fullName = $"{user.FirstName} {user.LastName}",
                level = (user.TotalPoints / 500) + 1, // Her 500 puanda bir seviye
                currentBadge = user.UserBadges.OrderByDescending(b => b.EarnedAt).FirstOrDefault()?.Badge?.BadgeName ?? "Yeni Çevreci",
                co2Saved = (totalWasteWeight * 2.5).ToString("N1") + " kg CO2",
                treeCount = Math.Floor(totalWasteWeight / 20) + " Ağaç",
                dailyGoalProgress = 65 // Bu kısım günlük görevler tablosundan çekilebilir
            });
        }

        // 2. İSTATİSTİKLER EKRANI VERİSİ
        [HttpGet("stats/{userId}")]
        public async Task<IActionResult> GetUserStats(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return NotFound();

            var history = await _transactionService.GetUserHistoryAsync(userId);
            var badges = await _context.UserBadges
                .Where(ub => ub.UserId == userId)
                .Select(ub => new { ub.Badge.BadgeName, ub.Badge.ImageUrl })
                .ToListAsync();

            return Ok(new
            {
                totalPoints = user.TotalPoints,
                trustScore = user.TrustScore,
                earnedBadges = badges,
                recentTransactions = history.Take(10) // Son 10 işlem
            });
        }
    }
}