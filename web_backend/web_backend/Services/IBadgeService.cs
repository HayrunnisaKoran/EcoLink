using web_backend.Models;

namespace web_backend.Services
{
    public interface IBadgeService
    {
        // Kullanıcının yeni rozet kazanıp kazanmadığını kontrol eder ve hak ediyorsa ekler
        Task<List<Badge>> CheckAndAwardBadgesAsync(int userId);
    }
}
