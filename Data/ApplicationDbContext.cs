using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UniversityTournamentPro.Models;

namespace UniversityTournamentPro.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Tournament> Tournaments { get; set; }
        public DbSet<Match> Matches { get; set; }
        public DbSet<Announcement> Announcements { get; set; }
        public DbSet<SiteSetting> SiteSettings { get; set; }
        public DbSet<TournamentSelection> TournamentSelections { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<Notification> Notifications { get; set; }
    }
}