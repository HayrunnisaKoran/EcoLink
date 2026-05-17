using Microsoft.AspNetCore.Mvc;
using web_backend.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;

namespace web_backend.Controllers.Apiler
{
    [Route("api/[controller]")] // Temel adres: api/Auth
    [ApiController]
    public class AuthController : ControllerBase // API olduğu için ControllerBase daha uygundur
    {
        private readonly AppDbContext _context;
        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        // MOBİL İÇİN GİRİŞ API'Sİ
        // Erişim Adresi: POST /api/Auth/login
        [HttpPost("login")]
        public async Task<IActionResult> LoginApi([FromBody] LoginViewModel loginDto)
        {
            if (loginDto == null || string.IsNullOrEmpty(loginDto.Email))
                return BadRequest(new { success = false, message = "E-posta veya şifre boş olamaz." });

            // Şimdilik düz metin kontrolü (Demo aşaması için)
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == loginDto.Email && u.PasswordHash == loginDto.Password);

            if (user != null)
            {
                // Mobil tarafın beklediği "Paket" (JSON)
                return Ok(new
                {
                    success = true,
                    userId = user.UserId,
                    firstName = user.FirstName,
                    role = user.Role,
                    message = "Giriş başarılı!"
                });
            }

            return Unauthorized(new { success = false, message = "E-posta veya şifre hatalı." });
        }

        // MOBİL İÇİN KAYIT API'Sİ
        // Erişim Adresi: POST /api/Auth/register
        [HttpPost("register")]
        public async Task<IActionResult> RegisterApi([FromBody] RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, errors = "Geçersiz veri girişi." });

            // Email kullanımda mı kontrolü
            var exists = await _context.Users.AnyAsync(u => u.Email == model.Email);
            if (exists) return BadRequest(new { success = false, message = "Bu e-posta adresi zaten kayıtlı." });

            var user = new User
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                PasswordHash = model.Password, // Demo sonrası hashlenmeli
                Role = "Student",
                CreatedAt = DateTime.Now,
                TrustScore = 100,
                TotalPoints = 0
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Kayıt başarılı!", userId = user.UserId });
        }
    }
}