using System.ComponentModel.DataAnnotations;

namespace UniversityTournamentPro.Models
{
    public class TournamentSelection
    {
        public int Id { get; set; }
        public string TeamName { get; set; }
        public string Faculty { get; set; } // Resimdeki hata: Geçersiz sütun
        public string TeamManager { get; set; }
        public string ManagerStudentNumber { get; set; }
        public string Phone { get; set; }
        public string? Email { get; set; }
        public string TeamPlayers { get; set; } // CS1061 hatasını çözer

        public int TournamentId { get; set; }
        public Tournament? Tournament { get; set; }
        public string? UserId { get; set; }
        public ApplicationUser? Student { get; set; }
        public DateTime SelectionDate { get; set; } = DateTime.Now;
        public string Status { get; set; } = "Onay Bekliyor";
    }
}