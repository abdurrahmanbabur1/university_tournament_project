namespace UniversityTournamentPro.Models
{
    public class Announcement
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public int? TournamentId { get; set; }
        public Tournament? Tournament { get; set; }
        public bool IsActive { get; set; } = true;
    }
}