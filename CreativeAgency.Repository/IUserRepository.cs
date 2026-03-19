using CreativeAgency.Models;

namespace CreativeAgency.Repository
{
    public interface IKorisnikRepozitorijum : IRepozitorijum<Korisnik>
    {
        Task<Korisnik?> PronadjiPoKorisnickomImenuAsync(string korisnickoIme);
        Task<Korisnik?> PronadjiPoEmailuAsync(string email);
        Task<bool> ValidirajAkreditiveAsync(string korisnickoIme, string lozinka);
    }
}
