using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net;
using System.Net.Mail;

namespace UniversityTournamentPro.Services
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // BURAYA KENDİ MAİLİMİZİ EKLEYECEĞİZ 
            var mail = "senin-mail-adresin@gmail.com";
            var pw = "google-uygulama-sifresi"; // Normal şifre değil, 'Uygulama Şifresi' olmalı

            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(mail, pw)
            };

            return client.SendMailAsync(
                new MailMessage(from: mail, to: email, subject, htmlMessage) { IsBodyHtml = true }
            );
        }
    }
}