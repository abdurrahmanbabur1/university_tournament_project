using System.Collections.Generic;

namespace UniversityTournamentPro.Models
{
    public class HomeViewModel
    {
        public List<Match> Matches { get; set; }
        public List<Tournament> Tournaments { get; set; } // Takvimde turnuva başlangıçlarını da göstermek için
        public List<Announcement> Announcements { get; set; }
        public List<Branch> Branches { get; set; } // Branş emojileri için
        public int PendingUserCount { get; set; } // Onay bekleyen öğrenci sayısı için
    }
}