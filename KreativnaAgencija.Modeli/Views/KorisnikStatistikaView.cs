using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CreativeAgency.Models.Views
{
    [Table("KorisnikStatistikaView")]
    public class KorisnikStatistikaView
    {
        [Key]
        public int KorisnikId { get; set; }
        public string KorisnickoIme { get; set; } = string.Empty;
        public string? PunoIme { get; set; }
        public string Email { get; set; } = string.Empty;
        public int BrojKreiranihProjеkata { get; set; }
        public int BrojKreiranihZadataka { get; set; }
        public int BrojDodjeljenihZadataka { get; set; }
        public int? BrojZavrsenihZadataka { get; set; }
        public int? BrojZadatakaUToku { get; set; }
        public int? BrojKasnihZadataka { get; set; }
    }
}
