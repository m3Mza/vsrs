using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CreativeAgency.Models.Views
{
    [Table("ZadaciKasneView")]
    public class ZadaciKasneView
    {
        [Key]
        public int ZadatakId { get; set; }
        public string ZadatakNaslov { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Prioritet { get; set; } = string.Empty;
        public DateTime? RokIzvrsenja { get; set; }
        public int? DanaKasni { get; set; }
        public int ProjekatId { get; set; }
        public string ProjekatNaziv { get; set; } = string.Empty;
        public string ProjekatKategorija { get; set; } = string.Empty;
        public int? DodeljenKorisnikId { get; set; }
        public string? DodeljenKorisnickoIme { get; set; }
        public string? DodeljenPunoIme { get; set; }
    }
}
