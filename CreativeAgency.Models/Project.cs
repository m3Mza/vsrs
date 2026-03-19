using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CreativeAgency.Models
{
    public class Projekat
    {
        [Key]
        public int ProjekatId { get; set; }

        [Required]
        [StringLength(200)]
        public string Naziv { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Opis { get; set; }

        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "Aktivan"; // Aktivan, Zavrsen, NaCekanju

        [Required]
        [StringLength(50)]
        public string Kategorija { get; set; } = "Sajt"; // Sajt, Aplikacija, Brending, Dizajn, Marketing

        public DateTime DatumPocetka { get; set; }

        public DateTime? DatumZavrsetka { get; set; }

        public DateTime DatumKreiranja { get; set; } = DateTime.UtcNow;

        public DateTime? DatumAzuriranja { get; set; }

        [ForeignKey("KreiraoKorisnik")]
        public int? KreiraoKorisnikId { get; set; }

        // Navigacione osobine
        public virtual Korisnik? KreiraoKorisnik { get; set; }
        public virtual ICollection<Zadatak> Zadaci { get; set; } = new List<Zadatak>();
    }
}
