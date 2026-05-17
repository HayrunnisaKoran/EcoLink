using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using web_backend.Models;

namespace web_backend.Controllers.Apiler
{
    [Route("api/[controller]")]
    [ApiController]
    public class RewardController : ControllerBase // View deðil veri dönmek için ControllerBase
    {
        private readonly AppDbContext _context;

        public RewardController(AppDbContext context)
        {
            _context = context;
        }

        // Android tarafý bu adresi arýyor: GET /api/Reward/list
        [HttpGet("list")]
        public async Task<IActionResult> GetBadges()
        {
            // Veritabanýndaki tüm rozetleri listeleriz
            var badges = await _context.Badges
                .Where(b => b.IsActive)
                .Select(b => new {
                    id = b.BadgeId,
                    name = b.BadgeName,
                    desc = b.Description,
                    reqPoints = b.RequiredPoints,
                    icon = b.IconName ?? "leaf_icon"
                })
                .ToListAsync();

            return Ok(badges); // Ýþte Android'in beklediði JSON paketi
        }
    }
}