using System.ServiceModel;
using CreativeAgency.Models;

namespace CreativeAgency.Services
{
    [ServiceContract]
    public interface IProjekatSoapServis
    {
        [OperationContract]
        Task<List<ProjekatDto>> PreuzmiSveProjekte();

        [OperationContract]
        Task<ProjekatDto?> PreuzmiProjekatPoId(int projekatId);

        [OperationContract]
        Task<ProjekatDto> KreirajProjekat(string naziv, string? opis, DateTime datumPocetka, int? kreiraoKorisnikId);

        [OperationContract]
        Task<bool> AzurirajStatusProjekta(int projekatId, string status);

        [OperationContract]
        Task<List<ZadatakDto>> PreuzmiZadatkePoProjetku(int projekatId);
    }

    public class ProjekatDto
    {
        public int ProjekatId { get; set; }
        public string Naziv { get; set; } = string.Empty;
        public string? Opis { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime DatumPocetka { get; set; }
        public DateTime? DatumZavrsetka { get; set; }
        public DateTime DatumKreiranja { get; set; }
        public int? KreiraoKorisnikId { get; set; }
        public int BrojZadataka { get; set; }
    }

    public class ZadatakDto
    {
        public int ZadatakId { get; set; }
        public int ProjekatId { get; set; }
        public string Naslov { get; set; } = string.Empty;
        public string? Opis { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Prioritet { get; set; } = string.Empty;
        public DateTime? RokIzvrsenja { get; set; }
        public DateTime DatumKreiranja { get; set; }
        public int? DodeljenKorisnikId { get; set; }
    }
}
