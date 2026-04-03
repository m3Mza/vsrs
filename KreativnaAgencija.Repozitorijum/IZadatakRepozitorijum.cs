using CreativeAgency.Models;

namespace CreativeAgency.Repository
{
    public interface IZadatakRepozitorijum : IRepozitorijum<Zadatak>
    {
        Task<IEnumerable<Zadatak>> PreuzmiZadatkePoProjetkuAsync(int projekatId);
        Task<IEnumerable<Zadatak>> PreuzmiZadatkePoKorisnikuAsync(int korisnikId);
        Task<IEnumerable<Zadatak>> PreuzmiZadatkeNaCekanjuAsync();
        Task<int> PrebrојAktivneZadatkePoKorisnikuAsync(int korisnikId);
    }
}
