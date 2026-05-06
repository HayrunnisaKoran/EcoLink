using Microsoft.EntityFrameworkCore;
using web_backend.Models;

namespace web_backend.Services
{
    public class RankingService : IRankingService
    {
        private readonly AppDbContext _context;

        public RankingService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<UserRankingDto>> GetTopUsersAsync(int count = 10)
        {
            var topUsers = await _context.Users
                .Where(u => u.IsActive)
                .OrderByDescending(u => u.TotalPoints) // En yüksek puan başa[cite: 4]
                .Take(count)
                .ToListAsync();

            return topUsers.Select((u, index) => new UserRankingDto
            {
                Rank = index + 1,
                FullName = $"{u.FirstName} {u.LastName}",
                TotalPoints = u.TotalPoints,
                TrustScore = u.TrustScore
            }).ToList();
        }

        public async Task<List<FacultyRankingDto>> GetFacultyRankingsAsync()
        {
            // Her fakültenin kullanıcılarının toplam puanını hesapla[cite: 2, 4]
            var facultyRankings = await _context.Faculties
                .Where(f => f.IsActive)
                .Select(f => new FacultyRankingDto
                {
                    FacultyName = f.FacultyName,
                    TotalPoints = _context.Users
                        .Where(u => u.FacultyId == f.FacultyId)
                        .Sum(u => u.TotalPoints)
                })
                .OrderByDescending(f => f.TotalPoints)
                .ToListAsync();

            for (int i = 0; i < facultyRankings.Count; i++)
            {
                facultyRankings[i].Rank = i + 1;
            }

            return facultyRankings;
        }
    }
}
