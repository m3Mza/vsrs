using System.ComponentModel.DataAnnotations;

namespace CreativeAgency.Web.ViewModels
{
    public class KreirajProjekatViewModel
    {
        [Required(ErrorMessage = "Naziv projekta je obavezan")]
        [StringLength(200, ErrorMessage = "Naziv ne može biti duži od 200 karaktera")]
        [Display(Name = "Naziv projekta")]
        public string Naziv { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Opis ne može biti duži od 1000 karaktera")]
        [Display(Name = "Opis")]
        public string? Opis { get; set; }

        [Required(ErrorMessage = "Status je obavezan")]
        [Display(Name = "Status")]
        public string Status { get; set; } = "Aktivan";

        [Required(ErrorMessage = "Kategorija je obavezna")]
        [Display(Name = "Kategorija")]
        public string Kategorija { get; set; } = "Sajt";

        [Required(ErrorMessage = "Datum početka je obavezan")]
        [Display(Name = "Datum početka")]
        [DataType(DataType.Date)]
        public DateTime DatumPocetka { get; set; } = DateTime.Now;
    }
}
