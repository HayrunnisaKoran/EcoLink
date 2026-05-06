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

            // 1. Kayıt klasörünü belirle (wwwroot/uploads/waste-photos)
            string uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "waste-photos");

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            // 2. Benzersiz dosya ismi oluştur (Çakışmaları önlemek için GUID kullanıyoruz)
            string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            // 3. Dosyayı fiziksel olarak kaydet
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            // 4. Veritabanına kaydedilecek bağıl yolu dön
            return "/uploads/waste-photos/" + uniqueFileName;
        }
    }
}