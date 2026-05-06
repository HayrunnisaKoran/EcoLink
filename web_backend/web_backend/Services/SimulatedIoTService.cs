using web_backend.Models;

namespace web_backend.Services
{
    public class SimulatedIoTService : IIoTService
    {
        private readonly AppDbContext _context;

        public SimulatedIoTService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> VerifyPhysicalDropAsync(int binId)
        {
            // Gerçek bir senaryoda burada ESP32/Arduino'ya HTTP isteği atılırdı.
            // Simülasyon için: Veritabanındaki kutu doluluk oranını küçük bir miktar artıralım.

            var bin = await _context.WasteBins.FindAsync(binId);
            if (bin == null) return false;

            // Rastgele bir hacim artışı simüle edelim (Örn: %0.5 - %1.5 arası)
            decimal simulatedIncrease = (decimal)(new Random().NextDouble() * (1.5 - 0.5) + 0.5);

            // Eğer kutu tamamen dolu değilse artışı yansıt
            if (bin.FillLevelPercent + simulatedIncrease <= 100)
            {
                bin.FillLevelPercent += simulatedIncrease;
                await _context.SaveChangesAsync();
                return true; // Fiziksel değişim onaylandı
            }

            return false;
        }
    }
}
