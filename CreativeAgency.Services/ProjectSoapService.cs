using CreativeAgency.Business;
using CreativeAgency.Models;

namespace CreativeAgency.Services
{
    public class ProjekatSoapServis : IProjekatSoapServis
    {
        private readonly ProjekatServis _projekatServis;
        private readonly ZadatakServis _zadatakServis;

        public ProjekatSoapServis(ProjekatServis projekatServis, ZadatakServis zadatakServis)
        {
            _projekatServis = projekatServis;
            _zadatakServis = zadatakServis;
        }

        public async Task<List<ProjekatDto>> PreuzmiSveProjekte()
        {
            var projekti = await _projekatServis.PreuzmiSveProjekteAsync();
            return projekti.Select(p => new ProjekatDto
            {
                ProjekatId = p.ProjekatId,
                Naziv = p.Naziv,
                Opis = p.Opis,
                Status = p.Status,
                DatumPocetka = p.DatumPocetka,
                DatumZavrsetka = p.DatumZavrsetka,
                DatumKreiranja = p.DatumKreiranja,
                KreiraoKorisnikId = p.KreiraoKorisnikId,
                BrojZadataka = p.Zadaci?.Count ?? 0
            }).ToList();
        }

        public async Task<ProjekatDto?> PreuzmiProjekatPoId(int projekatId)
        {
            var projekat = await _projekatServis.PreuzmiProjekatSaZadacimaAsync(projekatId);
            if (projekat == null) return null;

            return new ProjekatDto
            {
                ProjekatId = projekat.ProjekatId,
                Naziv = projekat.Naziv,
                Opis = projekat.Opis,
                Status = projekat.Status,
                DatumPocetka = projekat.DatumPocetka,
                DatumZavrsetka = projekat.DatumZavrsetka,
                DatumKreiranja = projekat.DatumKreiranja,
                KreiraoKorisnikId = projekat.KreiraoKorisnikId,
                BrojZadataka = projekat.Zadaci?.Count ?? 0
            };
        }

        public async Task<ProjekatDto> KreirajProjekat(string naziv, string? opis, DateTime datumPocetka, int? kreiraoKorisnikId)
        {
            var projekat = await _projekatServis.KreirajProjekatAsync(naziv, opis, datumPocetka, kreiraoKorisnikId);
            
            return new ProjekatDto
            {
                ProjekatId = projekat.ProjekatId,
                Naziv = projekat.Naziv,
                Opis = projekat.Opis,
                Status = projekat.Status,
                DatumPocetka = projekat.DatumPocetka,
                DatumZavrsetka = projekat.DatumZavrsetka,
                DatumKreiranja = projekat.DatumKreiranja,
                KreiraoKorisnikId = projekat.KreiraoKorisnikId,
                BrojZadataka = 0
            };
        }

        public async Task<bool> AzurirajStatusProjekta(int projekatId, string status)
        {
            var projekat = await _projekatServis.PreuzmiProjekatPoIdAsync(projekatId);
            if (projekat == null) return false;

            projekat.Status = status;
            await _projekatServis.AzurirajProjekatAsync(projekat);
            return true;
        }

        public async Task<List<ZadatakDto>> PreuzmiZadatkePoProjetku(int projekatId)
        {
            var zadaci = await _zadatakServis.PreuzmiZadatkePoProjetkuAsync(projekatId);
            return zadaci.Select(z => new ZadatakDto
            {
                ZadatakId = z.ZadatakId,
                ProjekatId = z.ProjekatId,
                Naslov = z.Naslov,
                Opis = z.Opis,
                Status = z.Status,
                Prioritet = z.Prioritet,
                RokIzvrsenja = z.RokIzvrsenja,
                DatumKreiranja = z.DatumKreiranja,
                DodeljenKorisnikId = z.DodeljenKorisnikId
            }).ToList();
        }
    }
}
