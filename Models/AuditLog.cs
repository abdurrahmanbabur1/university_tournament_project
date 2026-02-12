namespace UniversityTournamentPro.Models
{
    public class AuditLog
    {
        public int Id { get; set; }
        public string UserName { get; set; } // İşlemi yapan admin
        public string Action { get; set; } // Örn: "Takım Onaylandı", "Maç Silindi"
        public string Detail { get; set; } // Örn: "Mühendislik Gücü takımı onaylandı."
        public DateTime LogDate { get; set; } = DateTime.Now;
    }
}