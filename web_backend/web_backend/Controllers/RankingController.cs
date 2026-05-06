using Microsoft.AspNetCore.Mvc;
using web_backend.Services;

namespace web_backend.Controllers
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

        [HttpGet("users")]
        public async Task<IActionResult> GetUserRankings()
        {
            var rankings = await _rankingService.GetTopUsersAsync();
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