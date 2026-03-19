using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CreativeAgency.Models
{
    public class Korisnik
    {
        [Key]
        public int KorisnikId { get; set; }

        [Required]
        [StringLength(100)]
        public string KorisnickoIme { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string LozinkaHash { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Ime { get; set; }

        [StringLength(100)]
        public string? Prezime { get; set; }

        public DateTime DatumKreiranja { get; set; } = DateTime.UtcNow;

        public DateTime? DatumPoslednjegPrijave { get; set; }

        public bool JeAktivan { get; set; } = true;

        //Navigacione osobine
        public virtual ICollection<Projekat> KreiraniProjekti { get; set; } = new List<Projekat>();
        public virtual ICollection<Zadatak> KreiraniZadaci { get; set; } = new List<Zadatak>();
    }
}
