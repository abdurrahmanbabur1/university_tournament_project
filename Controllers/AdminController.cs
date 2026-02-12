using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversityTournamentPro.Data;
using UniversityTournamentPro.Models;

namespace UniversityTournamentPro.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;

        public AdminController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IEmailSender emailSender)
        {
            _context = context;
            _userManager = userManager;
            _emailSender = emailSender;
        }

        // --- 1. YARDIMCI METODLAR ---
        private async Task LogAction(string action, string detail)
        {
            var log = new AuditLog
            {
                UserName = User.Identity?.Name ?? "Bilinmeyen Admin",
                Action = action,
                Detail = detail,
                LogDate = DateTime.Now
            };
            _context.AuditLogs.Add(log);
            await _context.SaveChangesAsync();
        }

        // --- 2. GÖSTERGE PANELİ VE LOGLAR ---
        public async Task<IActionResult> Dashboard()
        {
            ViewBag.StudentCount = await _context.Users.CountAsync();
            ViewBag.TournamentCount = await _context.Tournaments.CountAsync();
            ViewBag.PendingApplications = await _context.TournamentSelections.CountAsync(s => s.Status == "Onay Bekliyor");
            ViewBag.MatchCount = await _context.Matches.CountAsync();
            ViewBag.PendingTeams = await _context.Teams.CountAsync(t => !t.IsApproved);
            ViewBag.PendingStudentCount = await _context.Users.CountAsync(u => !u.IsApproved);
            ViewBag.RecentLogs = await _context.AuditLogs.OrderByDescending(l => l.LogDate).Take(5).ToListAsync();

            return View();
        }

        public async Task<IActionResult> AuditLogs()
        {
            var logs = await _context.AuditLogs.OrderByDescending(l => l.LogDate).ToListAsync();
            return View(logs);
        }

        // --- 3. TURNUVA YÖNETİMİ ---
        public async Task<IActionResult> Tournaments()
        {
            var tournaments = await _context.Tournaments.ToListAsync();
            return View(tournaments);
        }

        [HttpGet]
        public async Task<IActionResult> CreateTournament()
        {
            ViewBag.Branches = await _context.Branches.ToListAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTournament(Tournament tournament)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tournament);
                await _context.SaveChangesAsync();
                await LogAction("Yeni Turnuva", $"'{tournament.Name}' oluşturuldu.");
                return RedirectToAction(nameof(Tournaments));
            }
            ViewBag.Branches = await _context.Branches.ToListAsync();
            return View(tournament);
        }

        [HttpGet]
        public async Task<IActionResult> EditTournament(int id)
        {
            var tournament = await _context.Tournaments.FindAsync(id);
            if (tournament == null) return NotFound();
            ViewBag.Branches = await _context.Branches.ToListAsync();
            return View(tournament);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditTournament(Tournament tournament)
        {
            if (ModelState.IsValid)
            {
                _context.Update(tournament);
                await _context.SaveChangesAsync();
                await LogAction("Turnuva Güncellendi", $"'{tournament.Name}' güncellendi.");
                return RedirectToAction(nameof(Tournaments));
            }
            ViewBag.Branches = await _context.Branches.ToListAsync();
            return View(tournament);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteTournament(int id)
        {
            var tournament = await _context.Tournaments.FindAsync(id);
            if (tournament != null)
            {
                string name = tournament.Name;
                _context.Tournaments.Remove(tournament);
                await _context.SaveChangesAsync();
                await LogAction("Turnuva Silindi", $"'{name}' kaldırıldı.");
            }
            return RedirectToAction(nameof(Tournaments));
        }

        // --- 4. BRANŞ YÖNETİMİ ---
        public async Task<IActionResult> ManageBranches()
        {
            var branches = await _context.Branches.ToListAsync();
            return View(branches);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateBranch(Branch branch)
        {
            if (ModelState.IsValid)
            {
                _context.Branches.Add(branch);
                await _context.SaveChangesAsync();
                await LogAction("Yeni Branş", $"'{branch.Name}' eklendi.");
                TempData["Success"] = "Yeni branş başarıyla eklendi!";
            }
            return RedirectToAction(nameof(ManageBranches));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteBranch(int id)
        {
            var branch = await _context.Branches.FindAsync(id);
            if (branch != null)
            {
                _context.Branches.Remove(branch);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Branş silindi.";
            }
            return RedirectToAction(nameof(ManageBranches));
        }

        // --- 5. MAÇ YÖNETİMİ ---
        public async Task<IActionResult> Matches()
        {
            var matches = await _context.Matches.Include(m => m.Tournament).OrderByDescending(m => m.MatchDate).ToListAsync();
            return View(matches);
        }

        [HttpGet]
        public async Task<IActionResult> CreateMatch()
        {
            ViewBag.Tournaments = await _context.Tournaments.Where(t => t.IsActive).ToListAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateMatch(Match match)
        {
            if (ModelState.IsValid)
            {
                _context.Matches.Add(match);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Maç başarıyla eklendi.";
                return RedirectToAction(nameof(Matches));
            }
            ViewBag.Tournaments = await _context.Tournaments.Where(t => t.IsActive).ToListAsync();
            return View(match);
        }

        // --- 6. ÖĞRENCİ YÖNETİMİ (ONAY, SİLME, DÜZENLE) ---
        public async Task<IActionResult> Students()
        {
            var students = await _context.Users.OrderByDescending(u => u.CreatedDate).ToListAsync();
            return View(students);
        }

        // --- 6. ÖĞRENCİ YÖNETİMİ (ONAY, SİLME, DÜZENLE) ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveStudent(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                TempData["Error"] = "Öğrenci bulunamadı.";
                return RedirectToAction(nameof(Students));
            }

            try
            {
                // 1. Kullanıcıyı Onayla
                user.IsApproved = true;
                var updateResult = await _userManager.UpdateAsync(user);

                if (updateResult.Succeeded)
                {
                    await LogAction("Öğrenci Onaylandı", $"{user.FirstName} {user.LastName} onaylandı.");

                    // 2. Bildirim Oluştur (Hata alsa da sistemi durdurmasın)
                    try
                    {
                        _context.Notifications.Add(new Notification
                        {
                            UserId = user.Email, // Email üzerinden eşleşiyorsa kalsın, ID ise user.Id yap
                            Message = "Hesabınız onaylanmıştır!",
                            CreatedAt = DateTime.Now
                        });
                        await _context.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        // Bildirim oluşmasa da devam et
                        Console.WriteLine("Bildirim hatası: " + ex.Message);
                    }

                    // 3. Mail Gönder (Mail ayarları bozuksa sistemi kilitler, o yüzden try-catch içinde)
                    try
                    {
                        await _emailSender.SendEmailAsync(user.Email, "MTÜ Turnuva - Onay",
                            $"Merhaba {user.FirstName}, hesabınız onaylandı. Artık giriş yapabilirsiniz.");
                    }
                    catch (Exception ex)
                    {
                        // Mail gitmese de admini bilgilendir ama işlemi iptal etme
                        TempData["Info"] = "Öğrenci onaylandı ancak bilgilendirme maili gönderilemedi.";
                    }

                    TempData["Success"] = "Öğrenci başarıyla onaylandı.";
                }
                else
                {
                    TempData["Error"] = "Kullanıcı güncellenirken bir hata oluştu.";
                }
            }
            catch (Exception)
            {
                TempData["Error"] = "Beklenmedik bir hata oluştu.";
            }

            // HER DURUMDA listeye geri dön, boş ekranda bırakma!
            return RedirectToAction(nameof(Students));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteStudent(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            try
            {
                // İlişkili verileri temizle (Cascade Delete simülasyonu)
                var selections = _context.TournamentSelections.Where(s => s.UserId == id);
                _context.TournamentSelections.RemoveRange(selections);

                var notifications = _context.Notifications.Where(n => n.UserId == user.Email);
                _context.Notifications.RemoveRange(notifications);

                await _context.SaveChangesAsync();

                // Kullanıcıyı sil
                var result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    await LogAction("Öğrenci Silindi", $"{user.FirstName} {user.LastName} silindi.");
                    TempData["Success"] = "Öğrenci ve tüm verileri silindi.";
                }
            }
            catch (Exception)
            {
                TempData["Error"] = "Silme işlemi sırasında bir hata oluştu.";
            }

            return RedirectToAction(nameof(Students));
        }

        [HttpGet]
        [HttpGet]
        public async Task<JsonResult> GetTournamentsForCalendar()
        {
            var tournamentList = await _context.Tournaments.ToListAsync();

            var events = tournamentList.Select(t => new
            {
                id = t.Id,
                title = "🏆 " + t.Name,
                // Saati boşverelim, sadece tarih gönderelim (YYYY-MM-DD)
                start = t.StartDate.ToString("yyyy-MM-dd"),
                description = t.Description,
                allDay = true
            });

            return Json(events);
        }

        // --- 7. TAKIM / TURNUVA BAŞVURU YÖNETİMİ ---
        public async Task<IActionResult> ManageTeams()
        {
            // Veritabanından başvuruları, turnuva ve öğrenci bilgileriyle beraber çekiyoruz
            var applications = await _context.TournamentSelections
                .Include(t => t.Tournament)
                .Include(t => t.Student) // ApplicationUser bilgilerini dahil et
                .OrderByDescending(t => t.SelectionDate)
                .ToListAsync();

            return View(applications);
        }

        // BAŞVURU DURUMUNU GÜNCELLEME (Onayla/Reddet)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateApplicationStatus(int id, string status)
        {
            var application = await _context.TournamentSelections.FindAsync(id);
            if (application != null)
            {
                application.Status = status;
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Başvuru durumu '{status}' olarak güncellendi.";

                await LogAction("Başvuru Güncellendi", $"ID:{id} nolu başvuru {status} yapıldı.");
            }
            return RedirectToAction(nameof(ManageTeams));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateTeamStatus(int teamId, bool approve)
        {
            var team = await _context.Teams.FindAsync(teamId);
            if (team != null)
            {
                team.IsApproved = approve;
                await _context.SaveChangesAsync();
                TempData["Success"] = "Takım durumu güncellendi.";
            }
            return RedirectToAction(nameof(ManageTeams));
        }

        // --- 8. AYARLAR VE DUYURULAR ---
        public async Task<IActionResult> ManageSettings()
        {
            var settings = await _context.SiteSettings.FirstOrDefaultAsync();
            return View(settings ?? new SiteSetting());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageSettings(SiteSetting model)
        {
            var existing = await _context.SiteSettings.FirstOrDefaultAsync();
            if (existing == null) _context.SiteSettings.Add(model);
            else
            {
                existing.UniversityName = model.UniversityName;
                existing.SksEmail = model.SksEmail;
                existing.SksPhone = model.SksPhone;
                existing.InstagramUrl = model.InstagramUrl;
                _context.Update(existing);
            }
            await _context.SaveChangesAsync();
            TempData["Success"] = "Ayarlar kaydedildi.";
            return RedirectToAction(nameof(Dashboard));
        }

        public async Task<IActionResult> Announcements()
        {
            return View(await _context.Announcements.OrderByDescending(a => a.CreatedDate).ToListAsync());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAnnouncement(Announcement announcement, IFormFile? imageFile)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/announcements", fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create)) { await imageFile.CopyToAsync(stream); }
                    announcement.ImageUrl = "/images/announcements/" + fileName;
                }
                announcement.CreatedDate = DateTime.Now;
                _context.Add(announcement);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Duyuru yayınlandı!";
            }
            return RedirectToAction(nameof(Announcements));
        }
    }
}