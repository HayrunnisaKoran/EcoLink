using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System;
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
        // Parametreleri temizledik, doğrudan WasteRecordRequest DTO'sunu alıyoruz
        public async Task<IActionResult> UploadWaste([FromForm] WasteRecordRequest request)
        {
            if (request == null || request.PhotoUrl == null)
                return BadRequest("Hata: Geçersiz istek veya fotoğraf eksik.");

            // 0. Anomali ve Suistimal Kontrolü (Rate Limiting)
            var isAnomaly = await _trustScoreService.IsAnomalyDetectedAsync(request.UserId);
            if (isAnomaly)
            {
                return BadRequest("Hata: Çok kısa sürede çok fazla işlem yaptınız. Lütfen bekleyin.");
            }

            // 1. Konum Doğrulaması (Geofencing)
            var lat = (decimal)request.Latitude;
            var lng = (decimal)request.Longitude;
            var isNear = await _binService.IsUserNearBinAsync(request.WasteBinId, lat, lng);
            if (!isNear) return BadRequest("Hata: Atık kutusunun başında olmalısınız (Max 5m).");

            // 2. Görüntü İşleme (AI Analizi)
            using var ms = new MemoryStream();
            await request.PhotoUrl.CopyToAsync(ms);
            var predictedLabel = await _predictionService.IdentifyWasteAsync(ms.ToArray());

            // 3. Kutu Türü ile Eşleşme Kontrolü
            var bin = await _context.WasteBins.FindAsync(request.WasteBinId);
            var wasteType = _context.WasteTypes.FirstOrDefault(t => t.TypeName == predictedLabel);

            if (wasteType == null || bin == null || bin.WasteTypeId != wasteType.WasteTypeId)
            {
                return BadRequest($"Hata: Yanlış kutu! Tespit edilen atık: {predictedLabel}.");
            }

            // IoT Sensör Tokalaşması (Handshaking)
            var sensorVerified = await _iotService.VerifyPhysicalDropAsync(request.WasteBinId);
            if (!sensorVerified)
            {
                return BadRequest("Hata: Kutuya atık girişi saptanamadı. Lütfen tekrar deneyin.");
            }

            // Dosya Saklama (Virüs/Magic Number kontrolü ile)
            var photoPath = await _fileStorageService.SaveFileAsync(request.PhotoUrl);

            // 4. EcoPoint Kazanma ve Kayıt
            var user = await _context.Users.FindAsync(request.UserId);
            if (user == null) return NotFound("Kullanıcı bulunamadı.");

            var record = new WasteRecord
            {
                UserId = request.UserId,
                WasteBinId = request.WasteBinId,
                WasteTypeId = wasteType.WasteTypeId,
                PhotoUrl = photoPath,
                EarnedPoints = wasteType.BasePoint,
                VerificationStatus = "Onaylandı",
                CreatedAt = DateTime.Now,
                GpsVerified = true,
                PhotoVerified = true,
                SensorVerified = true,
                Latitude = (decimal)request.Latitude,
                Longitude = (decimal)request.Longitude
            };

            _context.WasteRecords.Add(record);

            // KULLANICI PUANINI EKLİYORUZ (Eksik olan kritik satır)
            user.TotalPoints += wasteType.BasePoint;

            await _context.SaveChangesAsync();

            await _pointTransactionService.AddTransactionAsync(
                request.UserId,
                wasteType.BasePoint,
                "Kazanıldı",
                $"{predictedLabel} Atık Gönderimi",
                record.WasteRecordId
            );

            // TrustScore servisi üzerinden tek noktadan güncelleme
            await _trustScoreService.UpdateTrustScoreAsync(request.UserId, true);

            var earnedBadges = await _badgeService.CheckAndAwardBadgesAsync(request.UserId);

            return Ok(new
            {
                Message = "Tebrikler! Atık başarıyla ayrıştırıldı.",
                Points = wasteType.BasePoint,
                TotalPoints = user.TotalPoints,
                DetectedType = predictedLabel,
                NewBadges = earnedBadges.Select(b => b.BadgeName).ToList()
            });
        }
    }
}
