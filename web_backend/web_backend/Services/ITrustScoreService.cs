namespace web_backend.Services
{
    public interface ITrustScoreService
    {
        // Kullanıcının son işlemlerini kontrol ederek anomali (hızlı yükleme vb.) olup olmadığını belirler
        Task<bool> IsAnomalyDetectedAsync(int userId);

        // Başarılı veya şüpheli işleme göre güven skorunu günceller
        Task UpdateTrustScoreAsync(int userId, bool isVerified);
    }
}
