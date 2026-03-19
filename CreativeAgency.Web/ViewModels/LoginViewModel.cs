using System.ComponentModel.DataAnnotations;

namespace CreativeAgency.Web.ViewModels
{
    public class PrijavaViewModel
    {
        [Required(ErrorMessage = "Korisničko ime je obavezno")]
        [Display(Name = "Korisničko ime")]
        public string KorisnickoIme { get; set; } = string.Empty;

        [Required(ErrorMessage = "Lozinka je obavezna")]
        [DataType(DataType.Password)]
        [Display(Name = "Lozinka")]
        public string Lozinka { get; set; } = string.Empty;

        [Display(Name = "Zapamti me")]
        public bool ZapamtiMe { get; set; }
    }
}
