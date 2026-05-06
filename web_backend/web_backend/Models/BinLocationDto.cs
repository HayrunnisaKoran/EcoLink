namespace web_backend.Models
{
    public class BinLocationDto
    {
        public int Id { get; set; }
        public string BinCode { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public int WasteTypeId { get; set; } // Renkli ikonlar için (plastik, metal vb.)
    }
}
