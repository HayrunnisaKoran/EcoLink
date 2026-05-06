using Microsoft.EntityFrameworkCore;
using web_backend.Models;

namespace web_backend.Services
{
    public class BinService : IBinService
    {
        private readonly AppDbContext _context;

        public BinService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<BinLocationDto>> GetActiveBinsAsync()
        {
            return await _context.WasteBins
                .Where(b => b.IsActive)
                .Select(b => new BinLocationDto
                {
                    Id = b.WasteBinId,
                    BinCode = b.BinCode,
                    Latitude = b.Latitude ?? 0, // decimal(10,7) hassasiyeti korunur
                    Longitude = b.Longitude ?? 0,
                    WasteTypeId = b.WasteTypeId
                }).ToListAsync();
        }

        public async Task<bool> IsUserNearBinAsync(int binId, decimal userLat, decimal userLong)
        {
            var bin = await _context.WasteBins.FindAsync(binId);
            if (bin == null || bin.Latitude == null || bin.Longitude == null) return false;

            // Haversine Formülü: İki koordinat arası mesafeyi metre cinsinden hesaplar
            double distance = CalculateDistance(
                (double)userLat, (double)userLong,
                (double)bin.Latitude, (double)bin.Longitude);

            return distance <= 5; // 5 metre kısıtlaması
        }

        private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            var R = 6371e3; // Dünyanın yarıçapı (metre)
            var phi1 = lat1 * Math.PI / 180;
            var phi2 = lat2 * Math.PI / 180;
            var deltaPhi = (lat2 - lat1) * Math.PI / 180;
            var deltaLambda = (lon2 - lon1) * Math.PI / 180;

            var a = Math.Sin(deltaPhi / 2) * Math.Sin(deltaPhi / 2) +
                    Math.Cos(phi1) * Math.Cos(phi2) *
                    Math.Sin(deltaLambda / 2) * Math.Sin(deltaLambda / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return R * c; // Metre cinsinden mesafe
        }
    }
}
