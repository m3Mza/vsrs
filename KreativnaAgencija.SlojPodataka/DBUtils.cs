using MySqlConnector;
using Microsoft.EntityFrameworkCore;

namespace CreativeAgency.DAL
{

    public class DBUtils
    {
        private readonly string _konekcioniNiz;

        public DBUtils(ApplicationDbContext kontekst)
        {
            _konekcioniNiz = kontekst.Database.GetConnectionString()
                ?? throw new InvalidOperationException("Konekcioni niz nije konfigurisan.");
        }

        public async Task<MySqlConnection> DobijKonekcijuAsync()
        {
            var konekcija = new MySqlConnection(_konekcioniNiz);
            await konekcija.OpenAsync();
            return konekcija;
        }

        public async Task<List<T>> IzvrsiStoredProceduruAsync<T>(
            string nazivProcedure,
            MySqlParameter[] parametri,
            Func<MySqlDataReader, T> mapiraj)
        {
            var rezultati = new List<T>();
            var paramNizovi = string.Join(", ", parametri.Select(p => p.ParameterName));
            var sql = $"CALL {nazivProcedure}({paramNizovi})";

            await using var konekcija = await DobijKonekcijuAsync();
            await using var komanda = new MySqlCommand(sql, konekcija);
            komanda.Parameters.AddRange(parametri);

            await using var citac = await komanda.ExecuteReaderAsync();
            while (await citac.ReadAsync())
            {
                rezultati.Add(mapiraj(citac));
            }

            return rezultati;
        }

        /// <summary>
        /// Izvršava stored proceduru koja ne vraća rezultat (INSERT, UPDATE, DELETE).
        /// </summary>
        public async Task IzvrsiKomanduAsync(string sql, params MySqlParameter[] parametri)
        {
            await using var konekcija = await DobijKonekcijuAsync();
            await using var komanda = new MySqlCommand(sql, konekcija);
            komanda.Parameters.AddRange(parametri);
            await komanda.ExecuteNonQueryAsync();
        }

        /// <summary>
        /// Izvršava stored proceduru koja vraća jedan skalarni rezultat (npr. novokreirani ID).
        /// </summary>
        public async Task<object?> IzvrsiSkalarAsync(string sql, params MySqlParameter[] parametri)
        {
            await using var konekcija = await DobijKonekcijuAsync();
            await using var komanda = new MySqlCommand(sql, konekcija);
            komanda.Parameters.AddRange(parametri);
            return await komanda.ExecuteScalarAsync();
        }

        /// <summary>
        /// Kreira MySqlParameter sa proverom null vrednosti.
        /// </summary>
        public static MySqlParameter KreirajParametar(string naziv, object? vrednost)
        {
            return new MySqlParameter(naziv, vrednost ?? DBNull.Value);
        }
    }
}
