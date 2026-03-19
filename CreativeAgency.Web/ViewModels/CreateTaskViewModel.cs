using System.ComponentModel.DataAnnotations;

namespace CreativeAgency.Web.ViewModels
{
    public class KreirajZadatakViewModel
    {
        [Required]
        public int ProjekatId { get; set; }

        [Required(ErrorMessage = "Naslov zadatka je obavezan")]
        [StringLength(200, ErrorMessage = "Naslov ne može biti duži od 200 karaktera")]
        [Display(Name = "Naslov zadatka")]
        public string Naslov { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Opis ne može biti duži od 1000 karaktera")]
        [Display(Name = "Opis")]
        public string? Opis { get; set; }

        [Required]
        [Display(Name = "Prioritet")]
        public string Prioritet { get; set; } = "Srednji";

        [Display(Name = "Rok izvršenja")]
        [DataType(DataType.Date)]
        public DateTime? RokIzvrsenja { get; set; }

        [Display(Name = "Dodeli korisniku")]
        public int? DodeljenKorisnikId { get; set; }
    }
}
