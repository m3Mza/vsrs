using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CreativeAgency.Models.Views
{
    [Table("AktivniProjektiSaZadacimaView")]
    public class AktivniProjektiSaZadacimaView
    {
        [Key]
        public int ProjekatId { get; set; }
        public string ProjekatNaziv { get; set; } = string.Empty;
        public string Kategorija { get; set; } = string.Empty;
        public DateTime DatumPocetka { get; set; }
        public DateTime? DatumZavrsetka { get; set; }
        public string? KreatorKorisnickoIme { get; set; }
        public string? KreatorPunoIme { get; set; }
        public int UkupnoZadataka { get; set; }
        public int ZadaciZavrseni { get; set; }
        public int ZadaciUToku { get; set; }
        public int ZadaciNaCekanju { get; set; }
        public int ZadaciBlokirani { get; set; }
        public decimal? ProcenatZavrsenosti { get; set; }
    }
}
