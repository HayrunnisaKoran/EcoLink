namespace web_backend.Models
{
    public class PointTransactionDto
    {
        public int Points { get; set; }
        public string TransactionType { get; set; } // "Kazanıldı" veya "Harcandı"
        public string Reason { get; set; } // "Plastik Atık Onayı", "Rozet Bonusu" vb.
        public DateTime CreatedAt { get; set; }
    }
}
