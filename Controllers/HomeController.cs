using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversityTournamentPro.Data;
using UniversityTournamentPro.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace UniversityTournamentPro.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // --- ANA SAYFA: Maçlar, Duyurular ve Admin Uyarı Sayısı ---
        public async Task<IActionResult> Index()
        {
            var viewModel = new HomeViewModel
            {
                // Müsabakaları, bağlı oldukları turnuva bilgisiyle (branş, kontenjan vb.) çekiyoruz
                Matches = await _context.Matches
                    .Include(m => m.Tournament)
                    .OrderBy(m => m.MatchDate)
                    .ToListAsync(),

                // Turnuvaları da takvimde göstermek için çekiyoruz
                Tournaments = await _context.Tournaments
                    .Where(t => t.IsActive)
                    .OrderBy(t => t.StartDate)
                    .ToListAsync(),

                // Duyuruları bağlı oldukları turnuva bilgisiyle çekiyoruz
                Announcements = await _context.Announcements
                    .Include(a => a.Tournament)
                    .Where(a => a.IsActive)
                    .OrderByDescending(a => a.CreatedDate)
                    .Take(5)
                    .ToListAsync(),

                // Branşları emojileriyle birlikte çekiyoruz
                Branches = await _context.Branches.ToListAsync(),

                PendingUserCount = await _context.Users.CountAsync(u => !u.IsApproved)
            };

            return View(viewModel);
        }

        // --- TURNUVA DETAYLARI ---
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var tournament = await _context.Tournaments
                .FirstOrDefaultAsync(t => t.Id == id);

            if (tournament == null)
            {
                return NotFound();
            }

            return View(tournament);
        }

        // --- BRANŞA GÖRE TURNUVALAR ---
        public async Task<IActionResult> BranchTournaments(string branchName)
        {
            if (string.IsNullOrEmpty(branchName))
            {
                return RedirectToAction(nameof(Index));
            }

            var tournaments = await _context.Tournaments
                .Where(t => t.IsActive && t.Branch == branchName)
                .OrderBy(t => t.StartDate)
                .ToListAsync();

            ViewBag.BranchName = branchName;
            
            // Branş emojisini de bulalım
            var branch = await _context.Branches.FirstOrDefaultAsync(b => b.Name == branchName);
            ViewBag.BranchEmoji = branch?.Emoji ?? "🏆";

            return View(tournaments);
        }


        public IActionResult Privacy()
        {
            return View();
        }
    }
}