using CreativeAgency.Models;

namespace CreativeAgency.Repository
{
    public interface IProjekatRepozitorijum : IRepozitorijum<Projekat>
    {
        Task<IEnumerable<Projekat>> PreuzmiAktivneProjekteAsync();
        Task<IEnumerable<Projekat>> PreuzmiProjektePoKorisnikuAsync(int korisnikId);
        Task<Projekat?> PreuzmiProjekatSaZadacimaAsync(int projekatId);
    }
}
