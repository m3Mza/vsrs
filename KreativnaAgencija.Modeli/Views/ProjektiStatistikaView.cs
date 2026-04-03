using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CreativeAgency.Models.Views
{
    [Table("ProjektiStatistikaView")]
    public class ProjektiStatistikaView
    {
        [Key]
        public string Kategorija { get; set; } = string.Empty;
        public int BrojProjеkata { get; set; }
        public int? AktivniProjekti { get; set; }
        public int? ZavrseniProjekti { get; set; }
        public int? ProjektiNaCekanju { get; set; }
        public int UkupnoZadataka { get; set; }
        public decimal? ProsecnoZadatakaPoProjektu { get; set; }
        public decimal? ProcenatZavrsenihZadataka { get; set; }
    }
}
