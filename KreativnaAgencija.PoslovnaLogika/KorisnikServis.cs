using CreativeAgency.Models;
using CreativeAgency.Repository;

namespace CreativeAgency.Business
{
    public class KorisnikServis
    {
        private readonly IKorisnikRepozitorijum _korisnikRepozitorijum;

        public KorisnikServis(IKorisnikRepozitorijum korisnikRepozitorijum)
        {
            _korisnikRepozitorijum = korisnikRepozitorijum;
        }

        public async Task<Korisnik?> AutentifikujAsync(string korisnickoIme, string lozinka)
        {
            var jeValidan = await _korisnikRepozitorijum.ValidirajAkreditiveAsync(korisnickoIme, lozinka);
            if (!jeValidan) return null;

            var korisnik = await _korisnikRepozitorijum.PronadjiPoKorisnickomImenuAsync(korisnickoIme);
            if (korisnik != null)
            {
                korisnik.DatumPoslednjegPrijave = DateTime.UtcNow;
                await _korisnikRepozitorijum.AzurirajAsync(korisnik);
            }

            return korisnik;
        }

        public async Task<Korisnik?> RegistrujAsync(string korisnickoIme, string email, string lozinka, string? ime = null, string? prezime = null)
        {
            // Check if username already exists
            if (await _korisnikRepozitorijum.PronadjiPoKorisnickomImenuAsync(korisnickoIme) != null)
                throw new InvalidOperationException("Korisničko ime već postoji");

            // Check if email already exists
            if (await _korisnikRepozitorijum.PronadjiPoEmailuAsync(email) != null)
                throw new InvalidOperationException("Email već postoji");

            var korisnik = new Korisnik
            {
                KorisnickoIme = korisnickoIme,
                Email = email,
                LozinkaHash = BCrypt.Net.BCrypt.HashPassword(lozinka),
                Ime = ime,
                Prezime = prezime,
                DatumKreiranja = DateTime.UtcNow,
                JeAktivan = true
            };

            return await _korisnikRepozitorijum.DodajAsync(korisnik);
        }

        public async Task<Korisnik?> PreuzmiKorisnikaPoIdAsync(int korisnikId)
        {
            return await _korisnikRepozitorijum.PreuzmiPoIdAsync(korisnikId);
        }

        public async Task<Korisnik?> PreuzmiKorisnikaPoKorisnickomImenuAsync(string korisnickoIme)
        {
            return await _korisnikRepozitorijum.PronadjiPoKorisnickomImenuAsync(korisnickoIme);
        }

        public async Task<IEnumerable<Korisnik>> PreuzmiSveKorisnikeAsync()
        {
            return await _korisnikRepozitorijum.PreuzmiSveAsync();
        }

        public async Task AzurirajKorisnikaAsync(Korisnik korisnik)
        {
            await _korisnikRepozitorijum.AzurirajAsync(korisnik);
        }
    }
}
