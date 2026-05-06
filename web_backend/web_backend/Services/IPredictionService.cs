namespace web_backend.Services
{
    public interface IPredictionService
    {
        // Fotoğrafı analiz edip atık türünü (plastik, metal vb.) döndüren metod[cite: 3]
        Task<string> IdentifyWasteAsync(byte[] imageBytes);
    }
}