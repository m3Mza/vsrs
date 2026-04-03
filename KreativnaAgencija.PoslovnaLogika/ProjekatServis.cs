using CreativeAgency.Models;
using CreativeAgency.Repository;

namespace CreativeAgency.Business
{
    public class ProjekatServis
    {
        private readonly IProjekatRepozitorijum _projekatRepozitorijum;

        public ProjekatServis(IProjekatRepozitorijum projekatRepozitorijum)
        {
            _projekatRepozitorijum = projekatRepozitorijum;
        }

        public async Task<IEnumerable<Projekat>> PreuzmiSveProjekteAsync()
        {
            return await _projekatRepozitorijum.PreuzmiSveAsync();
        }

        public async Task<IEnumerable<Projekat>> PreuzmiAktivneProjekteAsync()
        {
            return await _projekatRepozitorijum.PreuzmiAktivneProjekteAsync();
        }

        public async Task<Projekat?> PreuzmiProjekatPoIdAsync(int projekatId)
        {
            return await _projekatRepozitorijum.PreuzmiPoIdAsync(projekatId);
        }

        public async Task<Projekat?> PreuzmiProjekatSaZadacimaAsync(int projekatId)
        {
            return await _projekatRepozitorijum.PreuzmiProjekatSaZadacimaAsync(projekatId);
        }

        public async Task<IEnumerable<Projekat>> PreuzmiProjektePoKorisnikuAsync(int korisnikId)
        {
            return await _projekatRepozitorijum.PreuzmiProjektePoKorisnikuAsync(korisnikId);
        }

        public async Task<Projekat> KreirajProjekatAsync(string naziv, string? opis, DateTime datumPocetka, int? kreiraoKorisnikId)
        {
            var projekat = new Projekat
            {
                Naziv = naziv,
                Opis = opis,
                Status = "Aktivan",
                DatumPocetka = datumPocetka,
                DatumKreiranja = DateTime.UtcNow,
                KreiraoKorisnikId = kreiraoKorisnikId
            };

            return await _projekatRepozitorijum.DodajAsync(projekat);
        }

        public async Task<Projekat> DodajProjekatAsync(Projekat projekat)
        {
            projekat.DatumKreiranja = DateTime.UtcNow;
            return await _projekatRepozitorijum.DodajAsync(projekat);
        }

        public async Task AzurirajProjekatAsync(Projekat projekat)
        {
            projekat.DatumAzuriranja = DateTime.UtcNow;
            await _projekatRepozitorijum.AzurirajAsync(projekat);
        }

        public async Task ObrisiProjekatAsync(int projekatId)
        {
            var projekat = await _projekatRepozitorijum.PreuzmiPoIdAsync(projekatId);
            if (projekat != null)
            {
                await _projekatRepozitorijum.ObrisiAsync(projekat);
            }
        }

        public async Task<int> PrebrојProjekteAsync()
        {
            return await _projekatRepozitorijum.PrebrојAsync();
        }
    }
}
