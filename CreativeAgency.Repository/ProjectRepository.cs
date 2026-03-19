using Microsoft.EntityFrameworkCore;
using CreativeAgency.Models;
using CreativeAgency.DAL;
using MySqlConnector;

namespace CreativeAgency.Repository
{
    public class ProjekatRepozitorijum : Repozitorijum<Projekat>, IProjekatRepozitorijum
    {
        public ProjekatRepozitorijum(ApplicationDbContext kontekst) : base(kontekst)
        {
        }

        public override async Task<Projekat?> PreuzmiPoIdAsync(int id)
        {
            var parametri = new[]
            {
                new MySqlParameter("@p_ProjekatId", id)
            };

            return await IzvrsiStoredProceduruJedan<Projekat>("sp_PreuzmiProjekatPoId", parametri);
        }

        public override async Task<IEnumerable<Projekat>> PreuzmiSveAsync()
        {
            return await IzvrsiStoredProceduru<Projekat>("sp_PreuzmiSveProjekte");
        }

        public override async Task<Projekat> DodajAsync(Projekat entitet)
        {
            var parametri = new[]
            {
                new MySqlParameter("@p_Naziv", entitet.Naziv),
                new MySqlParameter("@p_Opis", (object?)entitet.Opis ?? DBNull.Value),
                new MySqlParameter("@p_Status", entitet.Status),
                new MySqlParameter("@p_Kategorija", entitet.Kategorija),
                new MySqlParameter("@p_DatumPocetka", entitet.DatumPocetka),
                new MySqlParameter("@p_DatumZavrsetka", (object?)entitet.DatumZavrsetka ?? DBNull.Value),
                new MySqlParameter("@p_KreiraoKorisnikId", (object?)entitet.KreiraoKorisnikId ?? DBNull.Value)
            };

            var sql = "CALL sp_KreirajProjekat(@p_Naziv, @p_Opis, @p_Status, @p_Kategorija, @p_DatumPocetka, @p_DatumZavrsetka, @p_KreiraoKorisnikId)";
            
            await using var connection = _kontekst.Database.GetDbConnection();
            await connection.OpenAsync();
            
            await using var command = connection.CreateCommand();
            command.CommandText = sql;
            command.Parameters.AddRange(parametri);
            
            var result = await command.ExecuteScalarAsync();
            entitet.ProjekatId = Convert.ToInt32(result);

            return entitet;
        }

        public override async Task AzurirajAsync(Projekat entitet)
        {
            var parametri = new[]
            {
                new MySqlParameter("@p_ProjekatId", entitet.ProjekatId),
                new MySqlParameter("@p_Naziv", entitet.Naziv),
                new MySqlParameter("@p_Opis", (object?)entitet.Opis ?? DBNull.Value),
                new MySqlParameter("@p_Status", entitet.Status),
                new MySqlParameter("@p_Kategorija", entitet.Kategorija),
                new MySqlParameter("@p_DatumPocetka", entitet.DatumPocetka),
                new MySqlParameter("@p_DatumZavrsetka", (object?)entitet.DatumZavrsetka ?? DBNull.Value)
            };

            var sql = "CALL sp_AzurirajProjekat(@p_ProjekatId, @p_Naziv, @p_Opis, @p_Status, @p_Kategorija, @p_DatumPocetka, @p_DatumZavrsetka)";
            await _kontekst.Database.ExecuteSqlRawAsync(sql, parametri);
        }

        public override async Task ObrisiAsync(Projekat entitet)
        {
            var parametri = new[]
            {
                new MySqlParameter("@p_ProjekatId", entitet.ProjekatId)
            };

            var sql = "CALL sp_ObrisiProjekat(@p_ProjekatId)";
            await _kontekst.Database.ExecuteSqlRawAsync(sql, parametri);
        }

        public async Task<IEnumerable<Projekat>> PreuzmiAktivneProjekteAsync()
        {
            return await IzvrsiStoredProceduru<Projekat>("sp_PreuzmiAktivneProjekte");
        }

        public async Task<IEnumerable<Projekat>> PreuzmiProjektePoKorisnikuAsync(int korisnikId)
        {
            var parametri = new[]
            {
                new MySqlParameter("@p_KorisnikId", korisnikId)
            };

            return await IzvrsiStoredProceduru<Projekat>("sp_PreuzmiProjektePoKorisniku", parametri);
        }

        public async Task<Projekat?> PreuzmiProjekatSaZadacimaAsync(int projekatId)
        {
            var parametri = new[]
            {
                new MySqlParameter("@p_ProjekatId", projekatId)
            };

            Projekat? projekat = null;
            var zadaci = new List<Zadatak>();

            var sql = "CALL sp_PreuzmiProjekatSaZadacima(@p_ProjekatId)";
            
            await using var connection = _kontekst.Database.GetDbConnection();
            await connection.OpenAsync();
            
            await using var command = connection.CreateCommand();
            command.CommandText = sql;
            command.Parameters.AddRange(parametri);
            
            await using var reader = await command.ExecuteReaderAsync();
            
            // First result set: Project
            if (await reader.ReadAsync())
            {
                projekat = new Projekat
                {
                    ProjekatId = reader.GetInt32(reader.GetOrdinal("ProjekatId")),
                    Naziv = reader.GetString(reader.GetOrdinal("Naziv")),
                    Opis = reader.IsDBNull(reader.GetOrdinal("Opis")) ? null : reader.GetString(reader.GetOrdinal("Opis")),
                    Status = reader.GetString(reader.GetOrdinal("Status")),
                    Kategorija = reader.GetString(reader.GetOrdinal("Kategorija")),
                    DatumPocetka = reader.GetDateTime(reader.GetOrdinal("DatumPocetka")),
                    DatumZavrsetka = reader.IsDBNull(reader.GetOrdinal("DatumZavrsetka")) ? null : reader.GetDateTime(reader.GetOrdinal("DatumZavrsetka")),
                    DatumKreiranja = reader.GetDateTime(reader.GetOrdinal("DatumKreiranja")),
                    DatumAzuriranja = reader.IsDBNull(reader.GetOrdinal("DatumAzuriranja")) ? null : reader.GetDateTime(reader.GetOrdinal("DatumAzuriranja")),
                    KreiraoKorisnikId = reader.IsDBNull(reader.GetOrdinal("KreiraoKorisnikId")) ? null : reader.GetInt32(reader.GetOrdinal("KreiraoKorisnikId")),
                    KreiraoKorisnik = reader.IsDBNull(reader.GetOrdinal("KreiraoKorisnikId")) ? null : new Korisnik
                    {
                        KorisnikId = reader.GetInt32(reader.GetOrdinal("KreiraoKorisnikId")),
                        KorisnickoIme = reader.GetString(reader.GetOrdinal("KorisnickoIme")),
                        Ime = reader.GetString(reader.GetOrdinal("KreatorIme")),
                        Prezime = reader.GetString(reader.GetOrdinal("KreatorPrezime"))
                    }
                };
            }

            // Second result set: Tasks
            if (projekat != null)
            {
                // Move to next result set
                if (await reader.NextResultAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var zadatak = new Zadatak
                        {
                            ZadatakId = reader.GetInt32(reader.GetOrdinal("ZadatakId")),
                            ProjekatId = reader.GetInt32(reader.GetOrdinal("ProjekatId")),
                            Naslov = reader.GetString(reader.GetOrdinal("Naslov")),
                            Opis = reader.IsDBNull(reader.GetOrdinal("Opis")) ? null : reader.GetString(reader.GetOrdinal("Opis")),
                            Status = reader.GetString(reader.GetOrdinal("Status")),
                            Prioritet = reader.GetString(reader.GetOrdinal("Prioritet")),
                            RokIzvrsenja = reader.IsDBNull(reader.GetOrdinal("RokIzvrsenja")) ? null : reader.GetDateTime(reader.GetOrdinal("RokIzvrsenja")),
                            DatumKreiranja = reader.GetDateTime(reader.GetOrdinal("DatumKreiranja")),
                            DatumAzuriranja = reader.IsDBNull(reader.GetOrdinal("DatumAzuriranja")) ? null : reader.GetDateTime(reader.GetOrdinal("DatumAzuriranja")),
                            DodeljenKorisnikId = reader.IsDBNull(reader.GetOrdinal("DodeljenKorisnikId")) ? null : reader.GetInt32(reader.GetOrdinal("DodeljenKorisnikId")),
                            KreiraoKorisnikId = reader.IsDBNull(reader.GetOrdinal("KreiraoKorisnikId")) ? null : reader.GetInt32(reader.GetOrdinal("KreiraoKorisnikId"))
                        };

                        // Set Dodeljen user if exists
                        if (zadatak.DodeljenKorisnikId.HasValue)
                        {
                            zadatak.Dodeljen = new Korisnik
                            {
                                KorisnikId = zadatak.DodeljenKorisnikId.Value,
                                KorisnickoIme = reader.GetString(reader.GetOrdinal("DodeljenKorisnickoIme")),
                                Ime = reader.GetString(reader.GetOrdinal("DodeljenIme")),
                                Prezime = reader.GetString(reader.GetOrdinal("DodeljenPrezime"))
                            };
                        }

                        // Set Creator user if exists
                        if (zadatak.KreiraoKorisnikId.HasValue)
                        {
                            zadatak.KreiraoKorisnik = new Korisnik
                            {
                                KorisnikId = zadatak.KreiraoKorisnikId.Value,
                                KorisnickoIme = reader.GetString(reader.GetOrdinal("KreatorKorisnickoIme")),
                                Ime = reader.GetString(reader.GetOrdinal("KreatorIme")),
                                Prezime = reader.GetString(reader.GetOrdinal("KreatorPrezime"))
                            };
                        }

                        zadaci.Add(zadatak);
                    }
                }
                
                // Always set the Zadaci collection
                projekat.Zadaci = zadaci;
            }

            return projekat;
        }
    }
}
