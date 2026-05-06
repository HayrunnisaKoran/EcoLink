using web_backend.Models;

namespace web_backend.Services
{
    public interface IRankingService
    {
        // En yüksek puanlı ilk X kullanıcıyı getirir
        Task<List<UserRankingDto>> GetTopUsersAsync(int count = 10);

        // Fakülteler arası "Yeşil Lig" sıralamasını getirir
        Task<List<FacultyRankingDto>> GetFacultyRankingsAsync();
    }
}
