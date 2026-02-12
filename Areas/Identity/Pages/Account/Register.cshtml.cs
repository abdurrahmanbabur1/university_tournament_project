using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using UniversityTournamentPro.Models;
using UniversityTournamentPro.Data;

namespace UniversityTournamentPro.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly ApplicationDbContext _context;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            IUserStore<ApplicationUser> userStore,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Ad alanı zorunludur.")]
            public string FirstName { get; set; }

            [Required(ErrorMessage = "Soyad alanı zorunludur.")]
            public string LastName { get; set; }

            [Required(ErrorMessage = "Fakülte alanı zorunludur.")]
            public string Faculty { get; set; }

            [Required(ErrorMessage = "Bölüm alanı zorunludur.")]
            public string Department { get; set; }

            [Required(ErrorMessage = "Öğrenci No zorunludur.")]
            public string StudentNo { get; set; }

            [Required(ErrorMessage = "Cinsiyet seçimi zorunludur.")]
            public string Gender { get; set; }

            [Required(ErrorMessage = "E-posta adresi gereklidir.")]
            [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Telefon numarası zorunludur.")]
            [Phone(ErrorMessage = "Geçerli bir telefon numarası giriniz.")]
            [Display(Name = "Telefon Numarası")]
            public string PhoneNumber { get; set; }

            [Required(ErrorMessage = "Şifre gereklidir.")]
            [StringLength(100, ErrorMessage = "Şifre en az {2} karakter olmalıdır.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Compare("Password", ErrorMessage = "Şifreler uyuşmuyor.")]
            public string ConfirmPassword { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                var user = CreateUser();

                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);

                // --- SENİN MODELİNLE BİREBİR EŞLEME ---
                user.FirstName = Input.FirstName;
                user.LastName = Input.LastName;
                user.Faculty = Input.Faculty;
                user.Department = Input.Department;
                user.StudentNo = Input.StudentNo;
                user.Gender = Input.Gender;
                user.PhoneNumber = Input.PhoneNumber;
                user.IsApproved = false; // Varsayılan onaysız
                user.CreatedDate = DateTime.Now;

                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("Kullanıcı tüm detaylarıyla oluşturuldu.");

                    await _userManager.AddToRoleAsync(user, "Ogrenci");

                    // --- ADMİNE BİLGİLENDİRME MAİLİ GÖNDER ---
                    try 
                    {
                        var settings = _context.SiteSettings.FirstOrDefault();
                        var adminEmail = settings?.SksEmail ?? "zanababur99@gmail.com";
                        
                        string adminSubject = "Yeni Öğrenci Kayıt Başvurusu";
                        string adminMessage = $@"
                            <h3>Yeni Kayıt Bildirimi</h3>
                            <p>Sisteme yeni bir öğrenci kayıt olmak için başvurdu. Detaylar:</p>
                            <ul>
                                <li><b>Ad Soyad:</b> {user.FirstName} {user.LastName}</li>
                                <li><b>Öğrenci No:</b> {user.StudentNo}</li>
                                <li><b>Fakülte/Bölüm:</b> {user.Faculty} / {user.Department}</li>
                                <li><b>E-posta:</b> {user.Email}</li>
                                <li><b>Telefon:</b> {user.PhoneNumber}</li>
                            </ul>
                            <p>Lütfen admin panelinden onaylayınız.</p>";

                        await _emailSender.SendEmailAsync(adminEmail, adminSubject, adminMessage);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Admin kayıt bildirim maili gönderilemedi.");
                    }

                    TempData["Success"] = "Kaydınız başarıyla alındı. Admin onayı sonrası giriş yapabilirsiniz.";

                    return RedirectToAction("Index", "Home", new { area = "" });
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return Page();
        }

        private ApplicationUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<ApplicationUser>();
            }
            catch
            {
                throw new InvalidOperationException($"'{nameof(ApplicationUser)}' örneği oluşturulamadı.");
            }
        }

        private IUserEmailStore<ApplicationUser> GetEmailStore()
        {
            return (IUserEmailStore<ApplicationUser>)_userStore;
        }
    }
}