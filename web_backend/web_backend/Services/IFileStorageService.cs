namespace web_backend.Services
{
    public interface IFileStorageService
    {
        // Dosyayı kaydeder ve erişim yolunu (URL) döner
        Task<string> SaveFileAsync(IFormFile file);
    }
}
