namespace web_backend.Models
{
    public class UserRankingDto
    {
        public int Rank { get; set; }
        public string FullName { get; set; }
        public int TotalPoints { get; set; }
        public int TrustScore { get; set; }
    }

    public class FacultyRankingDto
    {
        public int Rank { get; set; }
        public string FacultyName { get; set; }
        public int TotalPoints { get; set; }
    }
}
