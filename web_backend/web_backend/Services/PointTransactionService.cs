using Microsoft.EntityFrameworkCore;
using web_backend.Models;

namespace web_backend.Services
{
    public class PointTransactionService : IPointTransactionService
    {
        private readonly AppDbContext _context;

        public PointTransactionService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<PointTransactionDto>> GetUserHistoryAsync(int userId)
        {
            return await _context.PointsTransactions
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.CreatedAt) // En yeni işlem en üstte
                .Select(t => new PointTransactionDto
                {
                    Points = t.Points,
                    TransactionType = t.TransactionType,
                    Reason = t.Reason,
                    CreatedAt = t.CreatedAt
                }).ToListAsync(); 
        }

        public async Task AddTransactionAsync(int userId, int points, string type, string reason, int? wasteRecordId = null)
        {
            // 1. İşlemi kaydet
            var transaction = new PointsTransaction
            {
                UserId = userId,
                WasteRecordId = wasteRecordId,
                TransactionType = type,
                Points = points,
                Reason = reason,
                CreatedAt = DateTime.Now
            };

            _context.PointsTransactions.Add(transaction);

            // 2. Kullanıcının toplam puanını merkezi olarak güncelle
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                user.TotalPoints += points; 
            }

            await _context.SaveChangesAsync();
        }
    }
}
