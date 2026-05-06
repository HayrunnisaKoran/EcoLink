using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Security.Cryptography;
using web_backend.Models;
using web_backend.Services;

namespace web_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WasteController : Controller
    {
        private readonly IPredictionService _predictionService;
        private readonly IBinService _binService;
        private readonly AppDbContext _context;
        private readonly IFileStorageService _fileStorageService;
        private readonly IIoTService _iotService;
        private readonly ITrustScoreService _trustScoreService;
        private readonly IBadgeService _badgeService;
        private readonly IPointTransactionService _pointTransactionService;
        public WasteController(IPredictionService predictionService, IBinService binService,
                             IFileStorageService fileStorageService, IIoTService iotService,
                             ITrustScoreService trustScoreService, IBadgeService badgeService,
                             IPointTransactionService pointTransactionService,
        AppDbContext context)
        {
            _predictionService = predictionService;
            _binService = binService;
            _fileStorageService = fileStorageService;
            _iotService = iotService;
            _trustScoreService = trustScoreService;
            _badgeService = badgeService;
             _pointTransactionService= pointTransactionService;
            _context = context;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadWaste([FromForm] int userId, [FromForm] int binId, [FromForm] decimal lat, [FromForm] decimal lng, IFormFile photo)
        {
            // 0. Anomali ve Suistimal Kontrolü (Rate Limiting)[cite: 2]
            var isAnomaly = await _trustScoreService.IsAnomalyDetectedAsync(userId);
            if (isAnomaly)
            {
                return BadRequest("Hata: Çok kısa sürede çok fazla işlem yaptınız. Lütfen bekleyin."); 
    }

            // 1. Konum Doğrulaması (Geofencing)
            var isNear = await _binService.IsUserNearBinAsync(binId, lat, lng);
            if (!isNear) return BadRequest("Hata: Atık kutusunun başında olmalısınız (Max 5m).");

            // 2. Görüntü İşleme (AI Analizi)
            using var ms = new MemoryStream();
            await photo.CopyToAsync(ms);
            var predictedLabel = await _predictionService.IdentifyWasteAsync(ms.ToArray());

            // 3. Kutu Türü ile Eşleşme Kontrolü
            var bin = await _context.WasteBins.FindAsync(binId);
            var wasteType = _context.WasteTypes.FirstOrDefault(t => t.TypeName == predictedLabel);

            if (wasteType == null || bin.WasteTypeId != wasteType.WasteTypeId)
            {
                return BadRequest($"Hata: Yanlış kutu! Tespit edilen atık: {predictedLabel}.");
            }

            //  IoT Sensör Tokalaşması (Handshaking)
            var sensorVerified = await _iotService.VerifyPhysicalDropAsync(binId);

            if (!sensorVerified)
            {
                // Eğer sensör değişim algılamazsa işlem "şüpheli" olarak kaydedilir[cite: 2]
                return BadRequest("Hata: Kutuya atık girişi saptanamadı. Lütfen tekrar deneyin.");
            }
            //Dosya Saklama(Artık her şey kesinleşti, diski kullanabiliriz)
            
            var photoPath = await _fileStorageService.SaveFileAsync(photo);

            // 4. EcoPoint Kazanma ve Kayıt
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return NotFound("Kullanıcı bulunamadı.");

            var record = new WasteRecord
            {
                UserId = userId,
                WasteBinId = binId,
                WasteTypeId = wasteType.WasteTypeId,
                PhotoUrl = photoPath,
                EarnedPoints = wasteType.BasePoint,
                VerificationStatus = "Onaylandı",
                CreatedAt = DateTime.Now,
                GpsVerified = true,
                PhotoVerified = true,
                SensorVerified = true

            };

            _context.WasteRecords.Add(record);
            await _context.SaveChangesAsync();

            await _pointTransactionService.AddTransactionAsync(
            userId,
            wasteType.BasePoint,
            "Kazanıldı",
             $"{predictedLabel} Atık Gönderimi",
            record.WasteRecordId
         );



            // TrustScore servisi üzerinden tek noktadan güncelleme
            await _trustScoreService.UpdateTrustScoreAsync(userId, true);

            var earnedBadges = await _badgeService.CheckAndAwardBadgesAsync(userId);

            return Ok(new
            {
                Message = "Tebrikler! Atık başarıyla ayrıştırıldı.",
                Points = wasteType.BasePoint,
                DetectedType = predictedLabel,
                NewBadges = earnedBadges.Select(b => b.BadgeName).ToList() // Kazanılan rozet isimlerini de gönderiyoruz
            });
        }
    }
}
