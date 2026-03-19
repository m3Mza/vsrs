using CreativeAgency.Models;
using CreativeAgency.Repository;

namespace CreativeAgency.Business
{
    public class ZadatakServis
    {
        private readonly IZadatakRepozitorijum _zadatakRepozitorijum;
        private const int MaksimalanBrojAktivnihZadataka = 5;

        public ZadatakServis(IZadatakRepozitorijum zadatakRepozitorijum)
        {
            _zadatakRepozitorijum = zadatakRepozitorijum;
        }

        public async Task<IEnumerable<Zadatak>> PreuzmiSveZadatkeAsync()
        {
            return await _zadatakRepozitorijum.PreuzmiSveAsync();
        }

        public async Task<Zadatak?> PreuzmiZadatakPoIdAsync(int zadatakId)
        {
            return await _zadatakRepozitorijum.PreuzmiPoIdAsync(zadatakId);
        }

        public async Task<IEnumerable<Zadatak>> PreuzmiZadatkePoProjetkuAsync(int projekatId)
        {
            return await _zadatakRepozitorijum.PreuzmiZadatkePoProjetkuAsync(projekatId);
        }

        public async Task<IEnumerable<Zadatak>> PreuzmiZadatkePoKorisnikuAsync(int korisnikId)
        {
            return await _zadatakRepozitorijum.PreuzmiZadatkePoKorisnikuAsync(korisnikId);
        }

        public async Task<IEnumerable<Zadatak>> PreuzmiZadatkeNaCekanjuAsync()
        {
            return await _zadatakRepozitorijum.PreuzmiZadatkeNaCekanjuAsync();
        }

        public async Task<Zadatak> KreirajZadatakAsync(
            int projekatId, 
            string naslov, 
            string? opis, 
            string prioritet = "Srednji",
            DateTime? rokIzvrsenja = null,
            int? dodeljenKorisnikId = null,
            int? kreiraoKorisnikId = null)
        {
            // Provera poslovnog pravila: korisnik ne može imati više od 5 aktivnih zadataka
            if (dodeljenKorisnikId.HasValue)
            {
                var brojAktivnihZadataka = await _zadatakRepozitorijum.PrebrојAktivneZadatkePoKorisnikuAsync(dodeljenKorisnikId.Value);
                if (brojAktivnihZadataka >= MaksimalanBrojAktivnihZadataka)
                {
                    throw new InvalidOperationException(
                        $"Nije moguće dodeliti zadatak jer je korisnik dostigao maksimalan broj aktivnih zadataka ({MaksimalanBrojAktivnihZadataka}).");
                }
            }

            var zadatak = new Zadatak
            {
                ProjekatId = projekatId,
                Naslov = naslov,
                Opis = opis,
                Status = "NaCekanju",
                Prioritet = prioritet,
                RokIzvrsenja = rokIzvrsenja,
                DodeljenKorisnikId = dodeljenKorisnikId,
                KreiraoKorisnikId = kreiraoKorisnikId,
                DatumKreiranja = DateTime.UtcNow
            };

            return await _zadatakRepozitorijum.DodajAsync(zadatak);
        }

        public async Task AzurirajZadatakAsync(Zadatak zadatak)
        {
            zadatak.DatumAzuriranja = DateTime.UtcNow;
            await _zadatakRepozitorijum.AzurirajAsync(zadatak);
        }

        public async Task ObrisiZadatakAsync(int zadatakId)
        {
            var zadatak = await _zadatakRepozitorijum.PreuzmiPoIdAsync(zadatakId);
            if (zadatak != null)
            {
                await _zadatakRepozitorijum.ObrisiAsync(zadatak);
            }
        }

        public async Task DodeliZadatakAsync(int zadatakId, int korisnikId)
        {
            // Provera poslovnog pravila: korisnik ne može imati više od 5 aktivnih zadataka
            var brojAktivnihZadataka = await _zadatakRepozitorijum.PrebrојAktivneZadatkePoKorisnikuAsync(korisnikId);
            if (brojAktivnihZadataka >= MaksimalanBrojAktivnihZadataka)
            {
                throw new InvalidOperationException(
                    $"Nije moguće dodeliti zadatak jer je korisnik dostigao maksimalan broj aktivnih zadataka ({MaksimalanBrojAktivnihZadataka}).");
            }

            var zadatak = await _zadatakRepozitorijum.PreuzmiPoIdAsync(zadatakId);
            if (zadatak != null)
            {
                zadatak.DodeljenKorisnikId = korisnikId;
                zadatak.DatumAzuriranja = DateTime.UtcNow;
                await _zadatakRepozitorijum.AzurirajAsync(zadatak);
            }
        }

        public async Task AzurirajStatusZadatkaAsync(int zadatakId, string status)
        {
            var zadatak = await _zadatakRepozitorijum.PreuzmiPoIdAsync(zadatakId);
            if (zadatak != null)
            {
                zadatak.Status = status;
                zadatak.DatumAzuriranja = DateTime.UtcNow;
                await _zadatakRepozitorijum.AzurirajAsync(zadatak);
            }
        }
    }
}
