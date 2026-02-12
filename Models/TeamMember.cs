namespace UniversityTournamentPro.Models
{
    public class TeamMember
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string StudentNumber { get; set; }
        public int JerseyNumber { get; set; }

        public int TeamId { get; set; }
        public virtual Team Team { get; set; }
    }
}