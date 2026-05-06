using web_backend.Models;

namespace web_backend.Services
{
    public interface IBinService
    {
        // Harita için tüm aktif kutuları döner
        Task<List<BinLocationDto>> GetActiveBinsAsync(); 

        // Geofencing: Kullanıcı kutuya 5 metre yakınında mı?
        Task<bool> IsUserNearBinAsync(int binId, decimal userLat, decimal userLong);
    }
}
