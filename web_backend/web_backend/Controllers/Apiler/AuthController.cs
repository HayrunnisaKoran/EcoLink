using Microsoft.AspNetCore.Mvc;
using web_backend.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace web_backend.Controllers.Apiler
{
    public class AuthController : Controller
    {
        private readonly AppDbContext _context;
        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email && u.PasswordHash == password);

            if (user != null) return RedirectToAction("Index", "Dashboard");

            ViewBag.Error = "E-posta veya şifre hatalı.";
            return View();
        }

        [HttpPost]
        public IActionResult Register()
        {
            return View();
        }
        public IActionResult Logout()
        {
            return RedirectToAction("Login", "Auth");
        }

        // MOBİL İÇİN GİRİŞ API'Sİ
        [HttpPost("api/auth/login")]
        public async Task<IActionResult> LoginApi([FromBody] LoginViewModel model)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == model.Email && u.PasswordHash == model.Password);

            if (user != null)
            {
                // Mobil uygulamaya kullanıcı ID'sini ve başarısını dönüyoruz
                return Ok(new { success = true, userId = user.UserId, firstName = user.FirstName });
            }
            return BadRequest(new { success = false, message = "E-posta veya şifre hatalı." });
        }

        // MOBİL İÇİN KAYIT API'Sİ
        [HttpPost("api/auth/register")]
        public async Task<IActionResult> RegisterApi([FromBody] RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new User
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    PasswordHash = model.Password, // Şimdilik düz metin
                    Role = "Student"
                };
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return Ok(new { success = true, message = "Kayıt başarılı!" });
            }
            return BadRequest(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors) });
        }
    }
}