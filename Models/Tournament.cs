using System.ComponentModel.DataAnnotations;
using System;

namespace UniversityTournamentPro.Models
{
    public class Tournament
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Turnuva adı zorunludur.")]
        [Display(Name = "Turnuva Adı")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Spor branşı zorunludur.")]
        [Display(Name = "Branş")]
        public string Branch { get; set; }

        [Display(Name = "Başlangıç Tarihi")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Display(Name = "Açıklama")]
        public string Description { get; set; }

        // 🔥 YENİ: Kontenjan Alanı
        [Required(ErrorMessage = "Kontenjan zorunludur.")]
        [Display(Name = "Kontenjan")]
        [Range(1, 500, ErrorMessage = "Kontenjan en az 1, en fazla 500 olabilir.")]
        public int Quota { get; set; }

        public bool IsActive { get; set; } = true;
    }
}