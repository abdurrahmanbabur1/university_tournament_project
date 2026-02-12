using System.Collections.Generic;

namespace UniversityTournamentPro.Models
{
    public class HomeViewModel
    {
        public List<Match> Matches { get; set; }
        public List<Announcement> Announcements { get; set; }
 public int PendingUserCount { get; set; } // Onay bekleyen öğrenci sayısı için
    }
}