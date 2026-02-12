using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversityTournamentPro.Data;
using UniversityTournamentPro.Models;

namespace UniversityTournamentPro.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // --- ANA SAYFA: Maçlar, Duyurular ve Admin Uyarý Sayýsý ---
        public async Task<IActionResult> Index()
        {
            var viewModel = new HomeViewModel
            {
                // Yaklaþan son 5 maçý çekiyoruz
                Matches = await _context.Matches
                    .Include(m => m.Tournament)
                    .OrderByDescending(m => m.MatchDate)
                    .Take(5)
                    .ToListAsync(),

                // --- GÜNCELLEME: Duyurularý baðlý olduklarý turnuva bilgisiyle çekiyoruz ---
                Announcements = await _context.Announcements
                    .Include(a => a.Tournament) // Duyuru-Turnuva köprüsünü burada kuruyoruz
                    .Where(a => a.IsActive)
                    .OrderByDescending(a => a.CreatedDate)
                    .Take(5)
                    .ToListAsync(),

                PendingUserCount = await _context.Users.CountAsync(u => !u.IsApproved)
            };

            return View(viewModel);
        }

        // --- TURNUVA DETAYLARI ---
        // Öðrenci duyuruya týkladýðýnda o duyurunun 'TournamentId'si buraya gelir
        public async Task<IActionResult> Details(int? id)
        {
            // Eðer ID gelmemiþse veya yanlýþ gelmiþse ana sayfaya geri yolla
            if (id == null)
            {
                return RedirectToAction(nameof(Index));
            }

            // Veritabanýnda o spesifik turnuvayý buluyoruz
            var tournament = await _context.Tournaments
                .FirstOrDefaultAsync(t => t.Id == id);

            // Eðer veritabanýnda böyle bir turnuva kalmamýþsa (silinmiþse) 404 ver
            if (tournament == null)
            {
                return NotFound();
            }

            // Bulduðumuz turnuvayý Details.cshtml sayfasýna "Model" olarak gönderiyoruz
            return View(tournament);
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}