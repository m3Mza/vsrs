using Microsoft.EntityFrameworkCore;
using CreativeAgency.Models;
using CreativeAgency.DAL;
using MySqlConnector;

namespace CreativeAgency.Repository
{
    public class KorisnikRepozitorijum : Repozitorijum<Korisnik>, IKorisnikRepozitorijum
    {
        public KorisnikRepozitorijum(ApplicationDbContext kontekst) : base(kontekst)
        {
        }

        public override async Task<Korisnik?> PreuzmiPoIdAsync(int id)
        {
            var parametri = new[]
            {
                new MySqlParameter("@p_KorisnikId", id)
            };

            return await IzvrsiStoredProceduruJedan<Korisnik>("sp_PreuzmiKorisnikaPoId", parametri);
        }

        public override async Task<IEnumerable<Korisnik>> PreuzmiSveAsync()
        {
            return await IzvrsiStoredProceduru<Korisnik>("sp_PreuzmiSveKorisnike");
        }

        public override async Task<Korisnik> DodajAsync(Korisnik entitet)
        {
            var parametri = new[]
            {
                new MySqlParameter("@p_KorisnickoIme", entitet.KorisnickoIme),
                new MySqlParameter("@p_Email", entitet.Email),
                new MySqlParameter("@p_LozinkaHash", entitet.LozinkaHash),
                new MySqlParameter("@p_Ime", (object?)entitet.Ime ?? DBNull.Value),
                new MySqlParameter("@p_Prezime", (object?)entitet.Prezime ?? DBNull.Value)
            };

            var sql = "CALL sp_KreirajKorisnika(@p_KorisnickoIme, @p_Email, @p_LozinkaHash, @p_Ime, @p_Prezime)";
            
            await using var connection = _kontekst.Database.GetDbConnection();
            await connection.OpenAsync();
            
            await using var command = connection.CreateCommand();
            command.CommandText = sql;
            command.Parameters.AddRange(parametri);
            
            var result = await command.ExecuteScalarAsync();
            entitet.KorisnikId = Convert.ToInt32(result);

            return entitet;
        }

        public override async Task AzurirajAsync(Korisnik entitet)
        {
            var parametri = new[]
            {
                new MySqlParameter("@p_KorisnikId", entitet.KorisnikId),
                new MySqlParameter("@p_KorisnickoIme", entitet.KorisnickoIme),
                new MySqlParameter("@p_Email", entitet.Email),
                new MySqlParameter("@p_Ime", (object?)entitet.Ime ?? DBNull.Value),
                new MySqlParameter("@p_Prezime", (object?)entitet.Prezime ?? DBNull.Value),
                new MySqlParameter("@p_JeAktivan", entitet.JeAktivan)
            };

            var sql = "CALL sp_AzurirajKorisnika(@p_KorisnikId, @p_KorisnickoIme, @p_Email, @p_Ime, @p_Prezime, @p_JeAktivan)";
            await _kontekst.Database.ExecuteSqlRawAsync(sql, parametri);
        }

        public override async Task ObrisiAsync(Korisnik entitet)
        {
            var parametri = new[]
            {
                new MySqlParameter("@p_KorisnikId", entitet.KorisnikId)
            };

            var sql = "CALL sp_ObrisiKorisnika(@p_KorisnikId)";
            await _kontekst.Database.ExecuteSqlRawAsync(sql, parametri);
        }

        public async Task<Korisnik?> PronadjiPoKorisnickomImenuAsync(string korisnickoIme)
        {
            var parametri = new[]
            {
                new MySqlParameter("@p_KorisnickoIme", korisnickoIme)
            };

            return await IzvrsiStoredProceduruJedan<Korisnik>("sp_PronadjiKorisnikaPoKorisnickomImenu", parametri);
        }

        public async Task<Korisnik?> PronadjiPoEmailuAsync(string email)
        {
            var parametri = new[]
            {
                new MySqlParameter("@p_Email", email)
            };

            return await IzvrsiStoredProceduruJedan<Korisnik>("sp_PronadjiKorisnikaPoEmailu", parametri);
        }

        public async Task<bool> ValidirajAkreditiveAsync(string korisnickoIme, string lozinka)
        {
            var korisnik = await PronadjiPoKorisnickomImenuAsync(korisnickoIme);
            if (korisnik == null) return false;

            // In production, use BCrypt or similar for password hashing
            // For now, this is a placeholder
            return BCrypt.Net.BCrypt.Verify(lozinka, korisnik.LozinkaHash);
        }
    }
}
