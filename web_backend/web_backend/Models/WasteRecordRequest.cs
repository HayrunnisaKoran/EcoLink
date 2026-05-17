using Microsoft.AspNetCore.Http;

namespace web_backend.Models
{
    public class WasteRecordRequest
    {
        public int UserId { get; set; }
        public int WasteBinId { get; set; }
        public int WasteTypeId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string PhotoBase64 { get; set; }
    }
}