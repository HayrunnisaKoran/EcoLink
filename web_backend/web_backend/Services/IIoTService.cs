namespace web_backend.Services
{
    public interface IIoTService
    {
        // Kutuda fiziksel bir hareket/hacim değişikliği olup olmadığını doğrular
        Task<bool> VerifyPhysicalDropAsync(int binId);
    }
}
