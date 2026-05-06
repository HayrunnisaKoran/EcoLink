using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using web_backend.Models;

namespace web_backend.Controllers
{
    [Route("api/[controller]")] // Hem API hem MVC controller olarak kullanacağız
    public class WasteBinController : Controller
    {
        private readonly AppDbContext _context;
        public WasteBinController(AppDbContext context)
        {
            _context = context;
        }

        // Web Paneli için liste sayfası
        // URL: /WasteBin/Index
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var bins = await _context.WasteBins.Include(b => b.WasteType).ToListAsync();
            return View(bins);
        }

        // HEM MOBİL HEM WEB HARİTASI İÇİN: Tüm kutuları JSON döner
        // URL: /api/WasteBin/all
        [HttpGet("all")]
        [Route("api/[controller]/all")]
        public async Task<IActionResult> GetAllBins()
        {
            var bins = await _context.WasteBins
                .Select(b => new {
                    b.WasteBinId,
                    b.Latitude,
                    b.Longitude,
                    b.FillLevelPercent,
                    WasteTypeName = b.WasteType.TypeName,
                    b.IsActive
                })
                .ToListAsync();

            return Ok(bins); // Mobil uygulama bu adresten veriyi alacak
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(WasteBin bin)
        {
            if (ModelState.IsValid)
            {
                _context.WasteBins.Add(bin);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(bin);
        }
        public IActionResult Details(int id)
        {
            ViewBag.BinId = id;
            return View();
        }
    }
}