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

        [Display(Name = "Turnuva Yeri")]
        public string? Location { get; set; }

        [Required(ErrorMessage = "Kontenjan zorunludur.")]
        [Display(Name = "Kontenjan")]
        [Range(1, 500, ErrorMessage = "Kontenjan en az 1, en fazla 500 olabilir.")]
        public int Quota { get; set; }

        [Required(ErrorMessage = "Minimum oyuncu sayısı zorunludur.")]
        [Display(Name = "Min Oyuncu")]
        [Range(1, 100, ErrorMessage = "En az 1 oyuncu olmalıdır.")]
        public int MinPlayerCount { get; set; } = 1;

        [Required(ErrorMessage = "Maksimum oyuncu sayısı zorunludur.")]
        [Display(Name = "Max Oyuncu")]
        [Range(1, 100, ErrorMessage = "En fazla 100 oyuncu olabilir.")]
        public int MaxPlayerCount { get; set; } = 1;

        [Display(Name = "Geçerli Cinsiyet")]
        public string GenderCategory { get; set; } = "Karma"; // "Erkek", "Kadın", "Karma"

        public bool IsActive { get; set; } = true;

    }
}