using Microsoft.EntityFrameworkCore;
using CreativeAgency.Models;
using CreativeAgency.DAL;
using MySqlConnector;

namespace CreativeAgency.Repository
{
    public class ZadatakRepozitorijum : Repozitorijum<Zadatak>, IZadatakRepozitorijum
    {
        public ZadatakRepozitorijum(ApplicationDbContext kontekst) : base(kontekst)
        {
        }

        public override async Task<Zadatak?> PreuzmiPoIdAsync(int id)
        {
            var parametri = new[]
            {
                new MySqlParameter("@p_ZadatakId", id)
            };

            return await IzvrsiStoredProceduruJedan<Zadatak>("sp_PreuzmiZadatakPoId", parametri);
        }

        public override async Task<IEnumerable<Zadatak>> PreuzmiSveAsync()
        {
            return await IzvrsiStoredProceduru<Zadatak>("sp_PreuzmiSveZadatke");
        }

        public override async Task<Zadatak> DodajAsync(Zadatak entitet)
        {
            var parametri = new[]
            {
                new MySqlParameter("@p_ProjekatId", entitet.ProjekatId),
                new MySqlParameter("@p_Naslov", entitet.Naslov),
                new MySqlParameter("@p_Opis", (object?)entitet.Opis ?? DBNull.Value),
                new MySqlParameter("@p_Status", entitet.Status),
                new MySqlParameter("@p_Prioritet", entitet.Prioritet),
                new MySqlParameter("@p_RokIzvrsenja", (object?)entitet.RokIzvrsenja ?? DBNull.Value),
                new MySqlParameter("@p_DodeljenKorisnikId", (object?)entitet.DodeljenKorisnikId ?? DBNull.Value),
                new MySqlParameter("@p_KreiraoKorisnikId", (object?)entitet.KreiraoKorisnikId ?? DBNull.Value)
            };

            var sql = "CALL sp_KreirajZadatak(@p_ProjekatId, @p_Naslov, @p_Opis, @p_Status, @p_Prioritet, @p_RokIzvrsenja, @p_DodeljenKorisnikId, @p_KreiraoKorisnikId)";
            
            await using var connection = _kontekst.Database.GetDbConnection();
            await connection.OpenAsync();
            
            await using var command = connection.CreateCommand();
            command.CommandText = sql;
            command.Parameters.AddRange(parametri);
            
            var result = await command.ExecuteScalarAsync();
            entitet.ZadatakId = Convert.ToInt32(result);

            return entitet;
        }

        public override async Task AzurirajAsync(Zadatak entitet)
        {
            var parametri = new[]
            {
                new MySqlParameter("@p_ZadatakId", entitet.ZadatakId),
                new MySqlParameter("@p_Naslov", entitet.Naslov),
                new MySqlParameter("@p_Opis", (object?)entitet.Opis ?? DBNull.Value),
                new MySqlParameter("@p_Status", entitet.Status),
                new MySqlParameter("@p_Prioritet", entitet.Prioritet),
                new MySqlParameter("@p_RokIzvrsenja", (object?)entitet.RokIzvrsenja ?? DBNull.Value),
                new MySqlParameter("@p_DodeljenKorisnikId", (object?)entitet.DodeljenKorisnikId ?? DBNull.Value)
            };

            var sql = "CALL sp_AzurirajZadatak(@p_ZadatakId, @p_Naslov, @p_Opis, @p_Status, @p_Prioritet, @p_RokIzvrsenja, @p_DodeljenKorisnikId)";
            await _kontekst.Database.ExecuteSqlRawAsync(sql, parametri);
        }

        public override async Task ObrisiAsync(Zadatak entitet)
        {
            var parametri = new[]
            {
                new MySqlParameter("@p_ZadatakId", entitet.ZadatakId)
            };

            var sql = "CALL sp_ObrisiZadatak(@p_ZadatakId)";
            await _kontekst.Database.ExecuteSqlRawAsync(sql, parametri);
        }

        public async Task<IEnumerable<Zadatak>> PreuzmiZadatkePoProjetkuAsync(int projekatId)
        {
            var parametri = new[]
            {
                new MySqlParameter("@p_ProjekatId", projekatId)
            };

            return await IzvrsiStoredProceduru<Zadatak>("sp_PreuzmiZadatkePoProjektu", parametri);
        }

        public async Task<IEnumerable<Zadatak>> PreuzmiZadatkePoKorisnikuAsync(int korisnikId)
        {
            var parametri = new[]
            {
                new MySqlParameter("@p_KorisnikId", korisnikId)
            };

            return await IzvrsiStoredProceduru<Zadatak>("sp_PreuzmiZadatkePoKorisniku", parametri);
        }

        public async Task<IEnumerable<Zadatak>> PreuzmiZadatkeNaCekanjuAsync()
        {
            return await IzvrsiStoredProceduru<Zadatak>("sp_PreuzmiZadatkeNaCekanju");
        }

        public async Task<int> PrebrојAktivneZadatkePoKorisnikuAsync(int korisnikId)
        {
            var zadaci = await PreuzmiZadatkePoKorisnikuAsync(korisnikId);
            return zadaci.Count(z => z.Status != "Zavrsen");
        }
    }
}
