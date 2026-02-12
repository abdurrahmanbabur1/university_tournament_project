namespace UniversityTournamentPro.Models
{
    public class Team
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string FacultyDepartment { get; set; }
        public string CaptainFullName { get; set; }
        public string CaptainEmail { get; set; }
        public string CaptainPhone { get; set; }
        public string CaptainStudentNumber { get; set; }
        public int TournamentId { get; set; }
        public virtual Tournament Tournament { get; set; }
        public bool IsApproved { get; set; }
        public DateTime CreatedDate { get; set; }
        public virtual List<TeamMember> Members { get; set; } = new List<TeamMember>();
    }
}