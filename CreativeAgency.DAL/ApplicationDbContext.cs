using Microsoft.EntityFrameworkCore;
using CreativeAgency.Models;
using CreativeAgency.Models.Views;

namespace CreativeAgency.DAL
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Entity Tables
        public DbSet<Korisnik> Korisnici { get; set; }
        public DbSet<Projekat> Projekti { get; set; }
        public DbSet<Zadatak> Zadaci { get; set; }

        // Database Views
        public DbSet<AktivniProjektiSaZadacimaView> AktivniProjektiSaZadacima { get; set; }
        public DbSet<KorisnikStatistikaView> KorisnikStatistika { get; set; }
        public DbSet<ProjektiStatistikaView> ProjektiStatistika { get; set; }
        public DbSet<ZadaciKasneView> ZadaciKasne { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Konfiguracija Korisnik entiteta
            modelBuilder.Entity<Korisnik>(entity =>
            {
                entity.HasIndex(e => e.KorisnickoIme).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();

                entity.HasMany(e => e.KreiraniProjekti)
                    .WithOne(e => e.KreiraoKorisnik)
                    .HasForeignKey(e => e.KreiraoKorisnikId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasMany(e => e.KreiraniZadaci)
                    .WithOne(e => e.KreiraoKorisnik)
                    .HasForeignKey(e => e.KreiraoKorisnikId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // Konfiguracija Projekat entiteta
            modelBuilder.Entity<Projekat>(entity =>
            {
                entity.HasMany(e => e.Zadaci)
                    .WithOne(e => e.Projekat)
                    .HasForeignKey(e => e.ProjekatId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Konfiguracija Zadatak entiteta
            modelBuilder.Entity<Zadatak>(entity =>
            {
                entity.HasOne(e => e.Dodeljen)
                    .WithMany()
                    .HasForeignKey(e => e.DodeljenKorisnikId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // Konfiguracija Views
            modelBuilder.Entity<AktivniProjektiSaZadacimaView>(entity =>
            {
                entity.ToView("AktivniProjektiSaZadacimaView");
                entity.HasKey(e => e.ProjekatId);
            });

            modelBuilder.Entity<KorisnikStatistikaView>(entity =>
            {
                entity.ToView("KorisnikStatistikaView");
                entity.HasKey(e => e.KorisnikId);
            });

            modelBuilder.Entity<ProjektiStatistikaView>(entity =>
            {
                entity.ToView("ProjektiStatistikaView");
                entity.HasKey(e => e.Kategorija);
            });

            modelBuilder.Entity<ZadaciKasneView>(entity =>
            {
                entity.ToView("ZadaciKasneView");
                entity.HasKey(e => e.ZadatakId);
            });

            // Početni podaci
            // Lozinka za admin korisnika je "admin123"
            modelBuilder.Entity<Korisnik>().HasData(
                new Korisnik
                {
                    KorisnikId = 1,
                    KorisnickoIme = "admin",
                    Email = "admin@creativeagency.com",
                    LozinkaHash = "$2a$11$vBwNZ0QyQJ7cK9qX8YZN7.sYdJx8J0kY6kQ0Wx8kQwO8yX8yX8yX8", // admin123
                    Ime = "Admin",
                    Prezime = "Korisnik",
                    DatumKreiranja = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    JeAktivan = true
                }
            );

            modelBuilder.Entity<Projekat>().HasData(
                new Projekat
                {
                    ProjekatId = 1,
                    Naziv = "Primer Projekta",
                    Opis = "Ovo je primer projekta za demonstraciju",
                    Status = "Aktivan",
                    DatumPocetka = new DateTime(2026, 1, 15, 0, 0, 0, DateTimeKind.Utc),
                    DatumKreiranja = new DateTime(2026, 1, 15, 0, 0, 0, DateTimeKind.Utc),
                    KreiraoKorisnikId = 1
                }
            );

            modelBuilder.Entity<Zadatak>().HasData(
                new Zadatak
                {
                    ZadatakId = 1,
                    ProjekatId = 1,
                    Naslov = "Primer Zadatka",
                    Opis = "Ovo je primer zadatka",
                    Status = "NaCekanju",
                    Prioritet = "Srednji",
                    DatumKreiranja = new DateTime(2026, 1, 15, 0, 0, 0, DateTimeKind.Utc),
                    KreiraoKorisnikId = 1
                }
            );
        }
    }
}
