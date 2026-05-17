using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using web_backend; // AppDbContext'in bulunduğu namespace'i kendi projene göre ayarla

namespace web_backend.Controllers.Apiler
{
    // Rota: https://...ngrok.dev/api/wastebin
    [Route("api/[controller]")]
    [ApiController]
    public class WasteBinController : ControllerBase
    {
        private readonly AppDbContext _context;

        // Veritabanı bağlantısını Constructor üzerinden (Dependency Injection) alıyoruz
        public WasteBinController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/wastebin
        // Tüm atık kutularının koordinatlarını liste olarak döndürür
        [HttpGet("all")]
        public async Task<IActionResult> GetAllBins()
        {
            // Veritabanından kutuları çekip, sadece mobilin ihtiyacı olan verileri JSON'a dönüştürüyoruz
            var bins = await _context.WasteBins
             .Include(b => b.WasteType) // Atık tipini de beraberinde getiriyoruz
              .Where(b => b.IsActive)    // Sadece aktif kutuları gönderelim
              .Select(b => new
        {
            Id = b.WasteBinId,
            Type = b.WasteType != null ? b.WasteType.TypeName : "Bilinmiyor",
            Latitude = b.Latitude,
            Longitude = b.Longitude,
            FillLevel = b.FillLevelPercent,
            Location = b.LocationName            // Mobilde kutunun nerede olduğunu göstermek için harika bir veri!
        })
                .ToListAsync();

            // Eğer sistemde hiç kutu yoksa boş liste veya hata dönmesin diye kontrol
            if (bins == null || !bins.Any())
            {
                return Ok(new { message = "Sistemde henüz kayıtlı atık kutusu bulunmuyor.", data = bins });
            }

            // Her şey yolundaysa 200 OK koduyla JSON verisini fırlat
            return Ok(bins);
        }
    }
}
