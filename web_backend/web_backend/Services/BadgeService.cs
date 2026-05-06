using Microsoft.EntityFrameworkCore;
using web_backend.Models;

namespace web_backend.Services
{
    public class BadgeService : IBadgeService
    {
        private readonly AppDbContext _context;

        public BadgeService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Badge>> CheckAndAwardBadgesAsync(int userId)
        {
            var newEarnedBadges = new List<Badge>();

            // 1. Kullanıcının güncel bilgilerini al
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return newEarnedBadges;

            // 2. Kullanıcının toplam attığı atık sayısını hesapla[cite: 4]
            var totalWasteCount = await _context.WasteRecords.CountAsync(r => r.UserId == userId);

            // 3. Kullanıcının henüz sahip olmadığı aktif rozetleri getir[cite: 4]
            var ownedBadgeIds = await _context.UserBadges
                .Where(ub => ub.UserId == userId)
                .Select(ub => ub.BadgeId)
                .ToListAsync();

            var eligibleBadges = await _context.Badges
                .Where(b => b.IsActive && !ownedBadgeIds.Contains(b.BadgeId))
                .ToListAsync();

            // 4. Şartları kontrol et[cite: 4]
            foreach (var badge in eligibleBadges)
            {
                bool meetsPointCondition = badge.RequiredPoints.HasValue && user.TotalPoints >= badge.RequiredPoints.Value;
                bool meetsCountCondition = badge.RequiredRecordCount.HasValue && totalWasteCount >= badge.RequiredRecordCount.Value;

                if (meetsPointCondition || meetsCountCondition)
                {
                    // Rozeti kullanıcıya ata[cite: 4]
                    _context.UserBadges.Add(new UserBadge
                    {
                        UserId = userId,
                        BadgeId = badge.BadgeId,
                        EarnedAt = DateTime.Now
                    });
                    newEarnedBadges.Add(badge);
                }
            }

            if (newEarnedBadges.Any())
            {
                await _context.SaveChangesAsync();
            }

            return newEarnedBadges;
        }
    }
}