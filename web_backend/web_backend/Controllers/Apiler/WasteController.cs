using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using web_backend.Models;
using web_backend.Services;

namespace web_backend.Controllers.Apiler // Klasör yapına uygun namespace
{
    [Route("api/[controller]")]
    [ApiController]
    public class WasteController : ControllerBase // Sadece ControllerBase kullanıyoruz
    {
        private readonly IPredictionService _predictionService;
        private readonly IBinService _binService;
        private readonly AppDbContext _context;
        private readonly IFileStorageService _fileStorageService;
        private readonly IIoTService _iotService;
        private readonly ITrustScoreService _trustScoreService;
        private readonly IBadgeService _badgeService;
        private readonly IPointTransactionService _pointTransactionService;

        //  AI Etiketleri ile Veritabanı isimlerini eşleştiren sözlük
        private static readonly Dictionary<string, string> _wasteTypeMapping = new Dictionary<string, string>
        {
            { "cam", "Cam" },
            { "kagit", "Kağıt" },   // labels.txt'deki "kagit" -> DB'deki "Kâğıt"
            { "metal", "Metal" },
            { "organik", "Organik" },
            { "plastik", "Plastik" }
        };

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
            _pointTransactionService = pointTransactionService;
            _context = context;
        }



        [HttpPost("upload")]
        public async Task<IActionResult> UploadWaste([FromBody] WasteRecordRequest request)
        {
            Console.WriteLine($"GELEN İSTEK -> Kullanıcı: {request.UserId}, KutuID: {request.WasteBinId}, AtıkID: {request.WasteTypeId}");
            // 1. İstek kontrolü
            if (request == null || string.IsNullOrEmpty(request.PhotoBase64))
                return BadRequest(new { success = false, message = "Geçersiz istek veya fotoğraf eksik." });

            // 0. Anomali Kontrolü
            var isAnomaly = await _trustScoreService.IsAnomalyDetectedAsync(request.UserId);
            if (isAnomaly) return BadRequest(new { success = false, message = "Hata: Çok fazla işlem yaptınız." });

            // 1. Konum Doğrulaması
            var isNear = await _binService.IsUserNearBinAsync(request.WasteBinId, (decimal)request.Latitude, (decimal)request.Longitude);
            if (!isNear) return BadRequest(new { success = false, message = "Hata: Atık kutusunun başında olmalısınız." });
            

           byte[] imageBytes;
            try { imageBytes = Convert.FromBase64String(request.PhotoBase64); }
            catch { return BadRequest(new { success = false, message = "Fotoğraf formatı hatalı." }); }

            // 2. AI Analizi
            var predictedLabel = await _predictionService.IdentifyWasteAsync(imageBytes);

            // --- KRİTİK HATA ÖNLEME 1: AI null dönerse ---
            if (string.IsNullOrEmpty(predictedLabel))
            {
                return BadRequest(new { success = false, message = "AI analizi sonuç üretemedi." });
            }

            // --- KRİTİK HATA ÖNLEME 2: Sözlükte olmayan bir etiket gelirse ---
            if (!_wasteTypeMapping.TryGetValue(predictedLabel.ToLower(), out string dbFriendlyName))
            {
                return BadRequest(new { success = false, message = $"Hata: Tanımlanamayan atık: {predictedLabel}" });
            }

            var bin = await _context.WasteBins.Include(b => b.WasteType).FirstOrDefaultAsync(x => x.WasteBinId == request.WasteBinId);
            var predictedWasteType = _context.WasteTypes.FirstOrDefault(t => t.TypeName.Trim() == dbFriendlyName.Trim());

            // LOG 2: Veritabanı ve AI ne diyor? (SORUN BURADA ÇIKACAK)
            Console.WriteLine("------------------------------------------");
            Console.WriteLine($"GELEN PAKET -> BinID: {request.WasteBinId}, SecilenTurID: {request.WasteTypeId}");
            Console.WriteLine($"AI TAHMİNİ: {dbFriendlyName} (TypeID: {predictedWasteType?.WasteTypeId})");
            Console.WriteLine($"KUTU DURUMU: ID {bin?.WasteBinId} nolu kutu aslında {bin?.WasteType?.TypeName} (TypeID: {bin?.WasteTypeId}) bekliyor.");

            Console.WriteLine($"Gelen Konum -> Enlem (Lat): {request.Latitude}, Boylam (Lon): {request.Longitude}");

            Console.WriteLine("------------------------------------------");

            if (predictedWasteType == null || bin == null || bin.WasteTypeId != predictedWasteType.WasteTypeId)
            {
                // LOG 3: Reddetme sebebini terminale yaz
                string sebep = bin == null ? "Kutu DB'de yok!" : $"Kutu {bin.WasteType?.TypeName} beklerken AI {dbFriendlyName} buldu.";
                Console.WriteLine($"!!! REDDEDİLDİ !!! Sebep: {sebep}");
                Console.WriteLine("------------------------------------------");
                return BadRequest(new { success = false, message = $"Hata: Yanlış kutu! Tespit edilen: {dbFriendlyName}." });
            }

            var sensorVerified = await _iotService.VerifyPhysicalDropAsync(request.WasteBinId);
            if (!sensorVerified) return BadRequest(new { success = true, message = "Hata: Kutuya atık girişi saptanamadı." });

            var photoPath = await _fileStorageService.SaveFileFromBytesAsync(imageBytes);
            var user = await _context.Users.FindAsync(request.UserId);
            if (user == null) return NotFound("Kullanıcı bulunamadı.");

            var record = new WasteRecord
            {
                UserId = request.UserId,
                WasteBinId = request.WasteBinId,
                WasteTypeId = predictedWasteType.WasteTypeId,
                PhotoUrl = photoPath,
                EarnedPoints = predictedWasteType.BasePoint,
                Amount = 0.5m,
                Unit = "kg",
                VerificationStatus = "Onaylandı",
                CreatedAt = DateTime.Now,
                GpsVerified = true,
                PhotoVerified = true,
                SensorVerified = true,
                Latitude = (decimal)request.Latitude,
                Longitude = (decimal)request.Longitude
            };

            _context.WasteRecords.Add(record);
            await _context.SaveChangesAsync();

            await _pointTransactionService.AddTransactionAsync(request.UserId, predictedWasteType.BasePoint, "Kazanıldı", $"{dbFriendlyName} Atık Gönderimi", record.WasteRecordId);
            await _trustScoreService.UpdateTrustScoreAsync(request.UserId, true);

            // --- KRİTİK HATA ÖNLEME 3: Rozet servisi null dönerse ---
            var earnedBadges = await _badgeService.CheckAndAwardBadgesAsync(request.UserId);
            var badgeNames = earnedBadges?.Select(b => b.BadgeName).ToList() ?? new List<string>();


            Console.WriteLine(">>> BAŞARILI: Kayıt yapıldı.");
            Console.WriteLine("------------------------------------------");
            return Ok(new
            {
                success = true,
                message = "Atık başarıyla ayrıştırıldı!",
                points = predictedWasteType.BasePoint,
                detectedType = dbFriendlyName,
                newBadges = badgeNames
            });
        }
    }
}
