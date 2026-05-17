using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using web_backend.Models;
using web_backend.Services;

namespace web_backend.Controllers.Apiler
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
        // Controllers/Apiler/UserApiController.cs

        [HttpGet("profile/{userId}")]
        public async Task<IActionResult> GetProfileData(int userId)
        {
            var user = await _context.Users
                .Include(u => u.Faculty)
                .Include(u => u.UserBadges).ThenInclude(ub => ub.Badge)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null) return NotFound();

            // Toplam Atık ve Kompost Katkısı Hesaplaması
            var userRecords = await _context.WasteRecords
                .Include(r => r.WasteBin)
                .Where(r => r.UserId == userId).ToListAsync();

            var totalWeight = userRecords.Sum(r => (double?)r.Amount) ?? 0;
            var compostCount = userRecords.Count(r => r.WasteBin != null && r.WasteBin.IsCompostUnit);
            var totalWasteCount = userRecords.Count;

            // Fakülte İçi Sıralama (Basit Sorgu)
            var rankCount = await _context.Users
                .Where(u => u.FacultyId == user.FacultyId && u.TotalPoints > user.TotalPoints)
                .CountAsync();

            int myRank = rankCount + 1;

            return Ok(new
            {
                fullName = $"{user.FirstName} {user.LastName}",
                facultyName = user.Faculty?.FacultyName ?? "Genel Katılımcı",
                points = user.TotalPoints,
                level = (user.TotalPoints / 500) + 1, // Örn: 1250 puan / 500 = 2.5 + 1 = Seviye 3
                progress = (user.TotalPoints % 500) * 100 / 500, // Seviye içindeki % ilerleme
                nextLevelPoints = 500,
                currentPointsInLevel = user.TotalPoints % 500,
                totalRecords = totalWasteCount,
                compostContribution = compostCount,
                totalWeight = totalWeight,
                rankCount = myRank,
                currentBadge = user.UserBadges.OrderByDescending(b => b.EarnedAt).FirstOrDefault()?.Badge?.BadgeName ?? "Yeni Çevreci"
            });
        }

        // UserApiController.cs içine eklenecek (Profile metodunun altına)

        [HttpGet("stats/{userId}")]
        public async Task<IActionResult> GetUserStats(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return NotFound();

            // Puan geçmişini servisten çekiyoruz
            var history = await _transactionService.GetUserHistoryAsync(userId);

            // Rozetleri çekiyoruz
            var badges = await _context.UserBadges
                .Where(ub => ub.UserId == userId)
                .Select(ub => new { ub.Badge.BadgeName, ub.Badge.ImageUrl })
                .ToListAsync();

            // Android tarafına giden "İstatistik Paketi"
            return Ok(new
            {
                totalPoints = user.TotalPoints,
                trustScore = user.TrustScore,
                earnedBadges = badges,
                recentTransactions = history.Take(10) // Son 10 puan hareketi
            });
        }


        [HttpPut("updatePoints/{userId}")]
        public async Task<IActionResult> UpdatePoints(int userId, [FromQuery] int points)
        {
            // 1. Kullanıcıyı bul
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return NotFound(new { message = "Kullanıcı bulunamadı." });

           
       

            // 3. PointsTransactions tablosuna işlem kaydı ekle
            // Not: Enjekte ettiğin _transactionService'i kullanıyoruz
            await _transactionService.AddTransactionAsync(
                userId,
                points,
                "Kazanıldı", // TransactionType
                "Günlük Görev Tamamlama", // Reason
                null // WasteRecordId (manuel olduğu için null)
            );

            // 4. Değişiklikleri veritabanına işle
            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = "Puan başarıyla güncellendi.",
                newTotalPoints = user.TotalPoints
            });
        }
    }
}