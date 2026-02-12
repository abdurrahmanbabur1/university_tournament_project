namespace UniversityTournamentPro.Models
{
    public class Match
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Branch { get; set; }
        public string TeamA { get; set; } // Bu eksikti
        public string TeamB { get; set; } // Bu eksikti
        public string Location { get; set; }
        public string Description { get; set; }
        public DateTime MatchDate { get; set; }
        public int TournamentId { get; set; }
        public virtual Tournament Tournament { get; set; }
    }
}