using web_backend.Models;

namespace web_backend.Services
{
    public interface IPointTransactionService
    {
        // Kullanıcının puan geçmişini listeler
        Task<List<PointTransactionDto>> GetUserHistoryAsync(int userId);

        // Yeni bir puan hareketi kaydeder ve kullanıcının toplam puanını günceller[cite: 2, 4]
        Task AddTransactionAsync(int userId, int points, string type, string reason, int? wasteRecordId = null);
    }
}
