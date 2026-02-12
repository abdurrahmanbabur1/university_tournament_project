using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversityTournamentPro.Data;
using UniversityTournamentPro.Models;

namespace UniversityTournamentPro.Controllers
{
    [Authorize(Roles = "Ogrenci")]
    public class StudentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public StudentController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // --- AKTİF TURNUVALARI LİSTELE ---
        public async Task<IActionResult> Index() => View(await _context.Tournaments.Where(t => t.IsActive).ToListAsync());

        // --- ADIM 1: TAKIM BİLGİLERİ FORMUNU GÖSTER (GET) ---
        [HttpGet]
        public async Task<IActionResult> Apply(int id)
        {
            var tournament = await _context.Tournaments.FindAsync(id);
            if (tournament == null) return NotFound();

            var selection = new TournamentSelection
            {
                TournamentId = id,
                Tournament = tournament
            };

            return View(selection);
        }

        // --- ADIM 2: FORM DOLDURULUP GÖNDERİLDİĞİNDE ÇALIŞAN KISIM (POST) ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Apply(TournamentSelection selection, List<string> PlayerNames, List<string> PlayerNumbers, List<string> PlayerJerseys)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var hasAlreadyApplied = await _context.TournamentSelections
                .AnyAsync(s => s.UserId == user.Id && s.TournamentId == selection.TournamentId);

            if (hasAlreadyApplied)
            {
                TempData["Error"] = "Bu turnuvaya zaten bir başvurunuz bulunmaktadır.";
                return RedirectToAction(nameof(MyApplications));
            }

            if (ModelState.IsValid)
            {
                var players = new List<string>();
                if (PlayerNames != null)
                {
                    for (int i = 0; i < PlayerNames.Count; i++)
                    {
                        if (!string.IsNullOrEmpty(PlayerNames[i]))
                        {
                            players.Add($"{PlayerNames[i]} | No:{PlayerNumbers[i]} | Forma:{PlayerJerseys[i]}");
                        }
                    }
                }

                selection.UserId = user.Id;
                selection.Faculty = user.Faculty;
                selection.ManagerStudentNumber = user.StudentNo;
                selection.Phone = user.PhoneNumber ?? "+905000000000";
                selection.TeamManager = $"{user.FirstName} {user.LastName}";
                selection.TeamPlayers = string.Join(";", players);
                selection.SelectionDate = DateTime.Now;
                selection.Status = "Onay Bekliyor";

                _context.TournamentSelections.Add(selection);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Takım başvurunuz başarıyla alındı!";
                return RedirectToAction(nameof(MyApplications));
            }

            selection.Tournament = await _context.Tournaments.FindAsync(selection.TournamentId);
            return View(selection);
        }

        // --- ÖĞRENCİNİN KENDİ BAŞVURULARI ---
        public async Task<IActionResult> MyApplications()
        {
            var userId = _userManager.GetUserId(User);
            return View(await _context.TournamentSelections
                .Include(s => s.Tournament)
                .Where(s => s.UserId == userId)
                .OrderByDescending(s => s.SelectionDate)
                .ToListAsync());
        }

        // --- YENİ: BAŞVURUYU İPTAL ETME (SİLME) ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelApplication(int id)
        {
            var userId = _userManager.GetUserId(User);

            // Güvenlik: Sadece bu öğrenciye ait olan başvuruyu bul
            var selection = await _context.TournamentSelections
                .FirstOrDefaultAsync(s => s.Id == id && s.UserId == userId);

            if (selection == null) return NotFound();

            _context.TournamentSelections.Remove(selection);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Başvurunuz başarıyla iptal edildi.";
            return RedirectToAction(nameof(MyApplications));
        }

        // --- PROFİL VE DİĞER İŞLEMLER ---
        public async Task<IActionResult> Profile() => View(await _userManager.GetUserAsync(User));

        [HttpGet] public IActionResult ChangePassword() => View();

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var user = await _userManager.GetUserAsync(User);
            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

            if (result.Succeeded)
            {
                await _signInManager.RefreshSignInAsync(user);
                TempData["Success"] = "Şifreniz başarıyla güncellendi.";
                return RedirectToAction(nameof(Profile));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(model);
        }
    }
}