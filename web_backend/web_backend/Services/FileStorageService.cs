using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace web_backend.Services
{
    public class FileStorageService : IFileStorageService
    {
        private readonly IWebHostEnvironment _environment;

        public FileStorageService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<string> SaveFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0) return null;

            // 1. SİBER GÜVENLİK KONTROLÜ: Dosya içeriği gerçekten resim mi?
            if (!await IsValidImageSignatureAsync(file))
            {
                throw new Exception("Güvenlik İhlali: Sadece resim dosyaları yüklenebilir!");
            }

            // 2. Kayıt klasörünü belirle
            string uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "waste-photos");

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            // 3. Dosya uzantısını koruyarak benzersiz isim oluştur
            string extension = Path.GetExtension(file.FileName); // .jpg veya .png alır
            string uniqueFileName = Guid.NewGuid().ToString() + extension;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            // 4. Dosyayı fiziksel olarak kaydet
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            // 5. Veritabanına kaydedilecek bağıl yolu dön
            return "/uploads/waste-photos/" + uniqueFileName;
        }

        // Magic Numbers (Dosya İmzası) Kontrol Fonksiyonu
        private async Task<bool> IsValidImageSignatureAsync(IFormFile file)
        {
            using (var stream = file.OpenReadStream())
            {
                var bytes = new byte[4];
                await stream.ReadAsync(bytes, 0, 4);

                // JPEG/JPG: FF D8 FF
                if (bytes[0] == 0xFF && bytes[1] == 0xD8 && bytes[2] == 0xFF) return true;

                // PNG: 89 50 4E 47
                if (bytes[0] == 0x89 && bytes[1] == 0x50 && bytes[2] == 0x4E && bytes[3] == 0x47) return true;

                return false;
            }
        }
    }
}