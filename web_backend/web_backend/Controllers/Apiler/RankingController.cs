using Microsoft.AspNetCore.Mvc;
using web_backend.Services;

namespace web_backend.Controllers.Apiler
{
    [Route("api/[controller]")]
    [ApiController]
    public class RankingController : ControllerBase
    {
        private readonly IRankingService _rankingService;

        public RankingController(IRankingService rankingService)
        {
            _rankingService = rankingService;
        }

        // Android'in aradýðý: GET /api/Ranking/top
        [HttpGet("top")]
        public async Task<IActionResult> GetTopUsers()
        {
            var rankings = await _rankingService.GetTopUsersAsync(10); // Ýlk 10 kiþiyi getir
            return Ok(rankings);
        }
        [HttpGet("faculties")]
        public async Task<IActionResult> GetFacultyRankings()
        {
            var rankings = await _rankingService.GetFacultyRankingsAsync();
            return Ok(rankings);
        }
    

    }
}