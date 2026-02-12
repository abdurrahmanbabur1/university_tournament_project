using Microsoft.AspNetCore.Identity;
using System;

namespace UniversityTournamentPro.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Faculty { get; set; }
        public string Department { get; set; }
        public string StudentNo { get; set; }
        public string Gender { get; set; }
        public bool IsApproved { get; set; } = false;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}