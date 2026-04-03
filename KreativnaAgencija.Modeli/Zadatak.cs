using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CreativeAgency.Models
{
    public class Zadatak
    {
        [Key]
        public int ZadatakId { get; set; }

        [Required]
        [ForeignKey("Projekat")]
        public int ProjekatId { get; set; }

        [Required]
        [StringLength(200)]
        public string Naslov { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Opis { get; set; }

        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "NaCekanju"; // NaCekanju, UraduJe, Zavrsen, Blokiran

        [StringLength(50)]
        public string Prioritet { get; set; } = "Srednji"; // Nizak, Srednji, Visok, Hitan

        public DateTime? RokIzvrsenja { get; set; }

        public DateTime DatumKreiranja { get; set; } = DateTime.UtcNow;

        public DateTime? DatumAzuriranja { get; set; }

        [ForeignKey("Dodeljen")]
        public int? DodeljenKorisnikId { get; set; }

        [ForeignKey("KreiraoKorisnik")]
        public int? KreiraoKorisnikId { get; set; }

        // Navigacione osobine
        public virtual Projekat Projekat { get; set; } = null!;
        public virtual Korisnik? Dodeljen { get; set; }
        public virtual Korisnik? KreiraoKorisnik { get; set; }
    }
}
