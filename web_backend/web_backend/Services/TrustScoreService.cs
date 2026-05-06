using Microsoft.EntityFrameworkCore;
using web_backend.Models;

namespace web_backend.Services
{
    public class TrustScoreService : ITrustScoreService
    {
        private readonly AppDbContext _context;
        private const int MaxUploadsPerMinute = 3; // 1 dakikada yapılabilecek maksimum işlem

        public TrustScoreService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> IsAnomalyDetectedAsync(int userId)
        {
            // Son 1 dakika içinde yapılan işlem sayısını kontrol et (Rate Limiting)
            var oneMinuteAgo = DateTime.Now.AddMinutes(-1);
            var recentUploadCount = await _context.WasteRecords
                .CountAsync(r => r.UserId == userId && r.CreatedAt >= oneMinuteAgo); 

            if (recentUploadCount >= MaxUploadsPerMinute)
            {
                return true; // Anomali saptandı
            }
            return false;
        }

        public async Task UpdateTrustScoreAsync(int userId, bool isVerified)
        {
            var user = await _context.Users.FindAsync(userId); 
            if (user == null) return;

            if (isVerified)
            {
                user.TrustScore += 5; // Başarılı doğrulamada skoru artır[cite: 2]
            }
            else
            {
                user.TrustScore -= 10; // Hatalı/Şüpheli işlemde skoru düşür[cite: 2]
            }

            await _context.SaveChangesAsync(); 
        }
    }
}
