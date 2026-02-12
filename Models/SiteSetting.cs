namespace UniversityTournamentPro.Models
{
    public class SiteSetting
    {
        public int Id { get; set; }
        public string? UniversityName { get; set; } = "Malatya Turgut Özal Üniversitesi";
        public string? SksAddress { get; set; }
        public string? SksPhone { get; set; }
        public string? SksPhone2 { get; set; }
        public string? SksPhone3 { get; set; }
        public string? SksEmail { get; set; }

        // Sosyal Medya Linkleri
        public string? InstagramUrl { get; set; }
        public string? TwitterUrl { get; set; }
        public string? YoutubeUrl { get; set; }
        public string? FacebookUrl { get; set; }
    }
}