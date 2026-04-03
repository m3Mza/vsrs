using System.ComponentModel.DataAnnotations;

namespace CreativeAgency.Web.ViewModels
{
    public class RegistracijaViewModel
    {
        [Required(ErrorMessage = "Korisničko ime je obavezno")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Korisničko ime mora imati između 3 i 100 karaktera")]
        [Display(Name = "Korisničko ime")]
        public string KorisnickoIme { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email je obavezan")]
        [EmailAddress(ErrorMessage = "Nevažeća email adresa")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Lozinka je obavezna")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Lozinka mora imati najmanje 6 karaktera")]
        [DataType(DataType.Password)]
        [Display(Name = "Lozinka")]
        public string Lozinka { get; set; } = string.Empty;

        [Display(Name = "Ime")]
        [StringLength(100)]
        public string? Ime { get; set; }

        [Display(Name = "Prezime")]
        [StringLength(100)]
        public string? Prezime { get; set; }
    }
}
