using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CreativeAgency.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddStoredProceduresAndViews : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ==================== STORED PROCEDURES FOR KORISNIK ====================
            
            // SP: Create Korisnik
            migrationBuilder.Sql(@"
                CREATE PROCEDURE sp_KreirajKorisnika(
                    IN p_KorisnickoIme VARCHAR(100),
                    IN p_Email VARCHAR(255),
                    IN p_LozinkaHash VARCHAR(255),
                    IN p_Ime VARCHAR(100),
                    IN p_Prezime VARCHAR(100)
                )
                BEGIN
                    INSERT INTO Korisnici (KorisnickoIme, Email, LozinkaHash, Ime, Prezime, DatumKreiranja, JeAktivan)
                    VALUES (p_KorisnickoIme, p_Email, p_LozinkaHash, p_Ime, p_Prezime, UTC_TIMESTAMP(), 1);
                    
                    SELECT LAST_INSERT_ID() AS KorisnikId;
                END;
            ");

            // SP: Get Korisnik by Id
            migrationBuilder.Sql(@"
                CREATE PROCEDURE sp_PreuzmiKorisnikaPoId(IN p_KorisnikId INT)
                BEGIN
                    SELECT * FROM Korisnici WHERE KorisnikId = p_KorisnikId;
                END;
            ");

            // SP: Get All Korisnici
            migrationBuilder.Sql(@"
                CREATE PROCEDURE sp_PreuzmiSveKorisnike()
                BEGIN
                    SELECT * FROM Korisnici ORDER BY DatumKreiranja DESC;
                END;
            ");

            // SP: Update Korisnik
            migrationBuilder.Sql(@"
                CREATE PROCEDURE sp_AzurirajKorisnika(
                    IN p_KorisnikId INT,
                    IN p_KorisnickoIme VARCHAR(100),
                    IN p_Email VARCHAR(255),
                    IN p_Ime VARCHAR(100),
                    IN p_Prezime VARCHAR(100),
                    IN p_JeAktivan BOOLEAN
                )
                BEGIN
                    UPDATE Korisnici
                    SET KorisnickoIme = p_KorisnickoIme,
                        Email = p_Email,
                        Ime = p_Ime,
                        Prezime = p_Prezime,
                        JeAktivan = p_JeAktivan
                    WHERE KorisnikId = p_KorisnikId;
                END;
            ");

            // SP: Delete Korisnik
            migrationBuilder.Sql(@"
                CREATE PROCEDURE sp_ObrisiKorisnika(IN p_KorisnikId INT)
                BEGIN
                    DELETE FROM Korisnici WHERE KorisnikId = p_KorisnikId;
                END;
            ");

            // SP: Get Korisnik by Username
            migrationBuilder.Sql(@"
                CREATE PROCEDURE sp_PronadjiKorisnikaPoKorisnickomImenu(IN p_KorisnickoIme VARCHAR(100))
                BEGIN
                    SELECT * FROM Korisnici WHERE KorisnickoIme = p_KorisnickoIme LIMIT 1;
                END;
            ");

            // SP: Get Korisnik by Email
            migrationBuilder.Sql(@"
                CREATE PROCEDURE sp_PronadjiKorisnikaPoEmailu(IN p_Email VARCHAR(255))
                BEGIN
                    SELECT * FROM Korisnici WHERE Email = p_Email LIMIT 1;
                END;
            ");

            // ==================== STORED PROCEDURES FOR PROJEKAT ====================
            
            // SP: Create Projekat
            migrationBuilder.Sql(@"
                CREATE PROCEDURE sp_KreirajProjekat(
                    IN p_Naziv VARCHAR(200),
                    IN p_Opis VARCHAR(1000),
                    IN p_Status VARCHAR(50),
                    IN p_Kategorija VARCHAR(50),
                    IN p_DatumPocetka DATETIME,
                    IN p_DatumZavrsetka DATETIME,
                    IN p_KreiraoKorisnikId INT
                )
                BEGIN
                    INSERT INTO Projekti (Naziv, Opis, Status, Kategorija, DatumPocetka, DatumZavrsetka, DatumKreiranja, KreiraoKorisnikId)
                    VALUES (p_Naziv, p_Opis, p_Status, p_Kategorija, p_DatumPocetka, p_DatumZavrsetka, UTC_TIMESTAMP(), p_KreiraoKorisnikId);
                    
                    SELECT LAST_INSERT_ID() AS ProjekatId;
                END;
            ");

            // SP: Get Projekat by Id
            migrationBuilder.Sql(@"
                CREATE PROCEDURE sp_PreuzmiProjekatPoId(IN p_ProjekatId INT)
                BEGIN
                    SELECT * FROM Projekti WHERE ProjekatId = p_ProjekatId;
                END;
            ");

            // SP: Get All Projekti
            migrationBuilder.Sql(@"
                CREATE PROCEDURE sp_PreuzmiSveProjekte()
                BEGIN
                    SELECT * FROM Projekti ORDER BY DatumKreiranja DESC;
                END;
            ");

            // SP: Update Projekat
            migrationBuilder.Sql(@"
                CREATE PROCEDURE sp_AzurirajProjekat(
                    IN p_ProjekatId INT,
                    IN p_Naziv VARCHAR(200),
                    IN p_Opis VARCHAR(1000),
                    IN p_Status VARCHAR(50),
                    IN p_Kategorija VARCHAR(50),
                    IN p_DatumPocetka DATETIME,
                    IN p_DatumZavrsetka DATETIME
                )
                BEGIN
                    UPDATE Projekti
                    SET Naziv = p_Naziv,
                        Opis = p_Opis,
                        Status = p_Status,
                        Kategorija = p_Kategorija,
                        DatumPocetka = p_DatumPocetka,
                        DatumZavrsetka = p_DatumZavrsetka,
                        DatumAzuriranja = UTC_TIMESTAMP()
                    WHERE ProjekatId = p_ProjekatId;
                END;
            ");

            // SP: Delete Projekat
            migrationBuilder.Sql(@"
                CREATE PROCEDURE sp_ObrisiProjekat(IN p_ProjekatId INT)
                BEGIN
                    DELETE FROM Projekti WHERE ProjekatId = p_ProjekatId;
                END;
            ");

            // SP: Get Active Projects
            migrationBuilder.Sql(@"
                CREATE PROCEDURE sp_PreuzmiAktivneProjekte()
                BEGIN
                    SELECT p.*, k.KorisnickoIme, k.Ime, k.Prezime
                    FROM Projekti p
                    LEFT JOIN Korisnici k ON p.KreiraoKorisnikId = k.KorisnikId
                    WHERE p.Status = 'Aktivan'
                    ORDER BY p.DatumKreiranja DESC;
                END;
            ");

            // SP: Get Projects by User
            migrationBuilder.Sql(@"
                CREATE PROCEDURE sp_PreuzmiProjektePoKorisniku(IN p_KorisnikId INT)
                BEGIN
                    SELECT p.*, 
                           (SELECT COUNT(*) FROM Zadaci z WHERE z.ProjekatId = p.ProjekatId) AS BrojZadataka
                    FROM Projekti p
                    WHERE p.KreiraoKorisnikId = p_KorisnikId
                    ORDER BY p.DatumKreiranja DESC;
                END;
            ");

            // SP: Get Project with Tasks
            migrationBuilder.Sql(@"
                CREATE PROCEDURE sp_PreuzmiProjekatSaZadacima(IN p_ProjekatId INT)
                BEGIN
                    -- Get Project
                    SELECT p.*, k.KorisnickoIme, k.Ime AS KreatorIme, k.Prezime AS KreatorPrezime
                    FROM Projekti p
                    LEFT JOIN Korisnici k ON p.KreiraoKorisnikId = k.KorisnikId
                    WHERE p.ProjekatId = p_ProjekatId;
                    
                    -- Get Tasks for Project
                    SELECT z.*, 
                           kd.KorisnickoIme AS DodeljenKorisnickoIme, kd.Ime AS DodeljenIme, kd.Prezime AS DodeljenPrezime,
                           kk.KorisnickoIme AS KreatorKorisnickoIme, kk.Ime AS KreatorIme, kk.Prezime AS KreatorPrezime
                    FROM Zadaci z
                    LEFT JOIN Korisnici kd ON z.DodeljenKorisnikId = kd.KorisnikId
                    LEFT JOIN Korisnici kk ON z.KreiraoKorisnikId = kk.KorisnikId
                    WHERE z.ProjekatId = p_ProjekatId
                    ORDER BY z.DatumKreiranja DESC;
                END;
            ");

            // ==================== STORED PROCEDURES FOR ZADATAK ====================
            
            // SP: Create Zadatak
            migrationBuilder.Sql(@"
                CREATE PROCEDURE sp_KreirajZadatak(
                    IN p_ProjekatId INT,
                    IN p_Naslov VARCHAR(200),
                    IN p_Opis VARCHAR(1000),
                    IN p_Status VARCHAR(50),
                    IN p_Prioritet VARCHAR(50),
                    IN p_RokIzvrsenja DATETIME,
                    IN p_DodeljenKorisnikId INT,
                    IN p_KreiraoKorisnikId INT
                )
                BEGIN
                    INSERT INTO Zadaci (ProjekatId, Naslov, Opis, Status, Prioritet, RokIzvrsenja, DodeljenKorisnikId, KreiraoKorisnikId, DatumKreiranja)
                    VALUES (p_ProjekatId, p_Naslov, p_Opis, p_Status, p_Prioritet, p_RokIzvrsenja, p_DodeljenKorisnikId, p_KreiraoKorisnikId, UTC_TIMESTAMP());
                    
                    SELECT LAST_INSERT_ID() AS ZadatakId;
                END;
            ");

            // SP: Get Zadatak by Id
            migrationBuilder.Sql(@"
                CREATE PROCEDURE sp_PreuzmiZadatakPoId(IN p_ZadatakId INT)
                BEGIN
                    SELECT z.*, 
                           p.Naziv AS ProjekatNaziv,
                           kd.KorisnickoIme AS DodeljenKorisnickoIme, kd.Ime AS DodeljenIme, kd.Prezime AS DodeljenPrezime,
                           kk.KorisnickoIme AS KreatorKorisnickoIme, kk.Ime AS KreatorIme, kk.Prezime AS KreatorPrezime
                    FROM Zadaci z
                    LEFT JOIN Projekti p ON z.ProjekatId = p.ProjekatId
                    LEFT JOIN Korisnici kd ON z.DodeljenKorisnikId = kd.KorisnikId
                    LEFT JOIN Korisnici kk ON z.KreiraoKorisnikId = kk.KorisnikId
                    WHERE z.ZadatakId = p_ZadatakId;
                END;
            ");

            // SP: Get All Zadaci
            migrationBuilder.Sql(@"
                CREATE PROCEDURE sp_PreuzmiSveZadatke()
                BEGIN
                    SELECT z.*, 
                           p.Naziv AS ProjekatNaziv,
                           kd.KorisnickoIme AS DodeljenKorisnickoIme, kd.Ime AS DodeljenIme, kd.Prezime AS DodeljenPrezime,
                           kk.KorisnickoIme AS KreatorKorisnickoIme, kk.Ime AS KreatorIme, kk.Prezime AS KreatorPrezime
                    FROM Zadaci z
                    LEFT JOIN Projekti p ON z.ProjekatId = p.ProjekatId
                    LEFT JOIN Korisnici kd ON z.DodeljenKorisnikId = kd.KorisnikId
                    LEFT JOIN Korisnici kk ON z.KreiraoKorisnikId = kk.KorisnikId
                    ORDER BY z.DatumKreiranja DESC;
                END;
            ");

            // SP: Update Zadatak
            migrationBuilder.Sql(@"
                CREATE PROCEDURE sp_AzurirajZadatak(
                    IN p_ZadatakId INT,
                    IN p_Naslov VARCHAR(200),
                    IN p_Opis VARCHAR(1000),
                    IN p_Status VARCHAR(50),
                    IN p_Prioritet VARCHAR(50),
                    IN p_RokIzvrsenja DATETIME,
                    IN p_DodeljenKorisnikId INT
                )
                BEGIN
                    UPDATE Zadaci
                    SET Naslov = p_Naslov,
                        Opis = p_Opis,
                        Status = p_Status,
                        Prioritet = p_Prioritet,
                        RokIzvrsenja = p_RokIzvrsenja,
                        DodeljenKorisnikId = p_DodeljenKorisnikId,
                        DatumAzuriranja = UTC_TIMESTAMP()
                    WHERE ZadatakId = p_ZadatakId;
                END;
            ");

            // SP: Delete Zadatak
            migrationBuilder.Sql(@"
                CREATE PROCEDURE sp_ObrisiZadatak(IN p_ZadatakId INT)
                BEGIN
                    DELETE FROM Zadaci WHERE ZadatakId = p_ZadatakId;
                END;
            ");

            // SP: Get Tasks by Project
            migrationBuilder.Sql(@"
                CREATE PROCEDURE sp_PreuzmiZadatkePoProjektu(IN p_ProjekatId INT)
                BEGIN
                    SELECT z.*, 
                           p.Naziv AS ProjekatNaziv,
                           kd.KorisnickoIme AS DodeljenKorisnickoIme, kd.Ime AS DodeljenIme, kd.Prezime AS DodeljenPrezime,
                           kk.KorisnickoIme AS KreatorKorisnickoIme, kk.Ime AS KreatorIme, kk.Prezime AS KreatorPrezime
                    FROM Zadaci z
                    LEFT JOIN Projekti p ON z.ProjekatId = p.ProjekatId
                    LEFT JOIN Korisnici kd ON z.DodeljenKorisnikId = kd.KorisnikId
                    LEFT JOIN Korisnici kk ON z.KreiraoKorisnikId = kk.KorisnikId
                    WHERE z.ProjekatId = p_ProjekatId
                    ORDER BY z.DatumKreiranja DESC;
                END;
            ");

            // SP: Get Tasks by User
            migrationBuilder.Sql(@"
                CREATE PROCEDURE sp_PreuzmiZadatkePoKorisniku(IN p_KorisnikId INT)
                BEGIN
                    SELECT z.*, 
                           p.Naziv AS ProjekatNaziv, p.Status AS ProjekatStatus
                    FROM Zadaci z
                    LEFT JOIN Projekti p ON z.ProjekatId = p.ProjekatId
                    WHERE z.DodeljenKorisnikId = p_KorisnikId
                    ORDER BY z.RokIzvrsenja ASC;
                END;
            ");

            // SP: Get Pending Tasks
            migrationBuilder.Sql(@"
                CREATE PROCEDURE sp_PreuzmiZadatkeNaCekanju()
                BEGIN
                    SELECT z.*, 
                           p.Naziv AS ProjekatNaziv,
                           kd.KorisnickoIme AS DodeljenKorisnickoIme, kd.Ime AS DodeljenIme, kd.Prezime AS DodeljenPrezime
                    FROM Zadaci z
                    LEFT JOIN Projekti p ON z.ProjekatId = p.ProjekatId
                    LEFT JOIN Korisnici kd ON z.DodeljenKorisnikId = kd.KorisnikId
                    WHERE z.Status IN ('NaCekanju', 'UraduJe')
                    ORDER BY z.RokIzvrsenja ASC;
                END;
            ");

            // ==================== SQL VIEWS ====================
            
            // View: Active Projects with Task Statistics
            migrationBuilder.Sql(@"
                CREATE VIEW AktivniProjektiSaZadacimaView AS
                SELECT 
                    p.ProjekatId,
                    p.Naziv AS ProjekatNaziv,
                    p.Kategorija,
                    p.DatumPocetka,
                    p.DatumZavrsetka,
                    k.KorisnickoIme AS KreatorKorisnickoIme,
                    CONCAT(k.Ime, ' ', k.Prezime) AS KreatorPunoIme,
                    COUNT(z.ZadatakId) AS UkupnoZadataka,
                    SUM(CASE WHEN z.Status = 'Zavrsen' THEN 1 ELSE 0 END) AS ZadaciZavrseni,
                    SUM(CASE WHEN z.Status = 'UraduJe' THEN 1 ELSE 0 END) AS ZadaciUToku,
                    SUM(CASE WHEN z.Status = 'NaCekanju' THEN 1 ELSE 0 END) AS ZadaciNaCekanju,
                    SUM(CASE WHEN z.Status = 'Blokiran' THEN 1 ELSE 0 END) AS ZadaciBlokirani,
                    ROUND(
                        (SUM(CASE WHEN z.Status = 'Zavrsen' THEN 1 ELSE 0 END) * 100.0) / 
                        NULLIF(COUNT(z.ZadatakId), 0), 2
                    ) AS ProcenatZavrsenosti
                FROM Projekti p
                LEFT JOIN Korisnici k ON p.KreiraoKorisnikId = k.KorisnikId
                LEFT JOIN Zadaci z ON p.ProjekatId = z.ProjekatId
                WHERE p.Status = 'Aktivan'
                GROUP BY p.ProjekatId, p.Naziv, p.Kategorija, p.DatumPocetka, p.DatumZavrsetka, 
                         k.KorisnickoIme, k.Ime, k.Prezime;
            ");

            // View: User Performance Statistics
            migrationBuilder.Sql(@"
                CREATE VIEW KorisnikStatistikaView AS
                SELECT 
                    k.KorisnikId,
                    k.KorisnickoIme,
                    CONCAT(k.Ime, ' ', k.Prezime) AS PunoIme,
                    k.Email,
                    COUNT(DISTINCT p.ProjekatId) AS BrojKreiranihProjеkata,
                    COUNT(DISTINCT zk.ZadatakId) AS BrojKreiranihZadataka,
                    COUNT(DISTINCT zd.ZadatakId) AS BrojDodjeljenihZadataka,
                    SUM(CASE WHEN zd.Status = 'Zavrsen' THEN 1 ELSE 0 END) AS BrojZavrsenihZadataka,
                    SUM(CASE WHEN zd.Status = 'UraduJe' THEN 1 ELSE 0 END) AS BrojZadatakaUToku,
                    SUM(CASE WHEN zd.RokIzvrsenja < UTC_TIMESTAMP() AND zd.Status != 'Zavrsen' THEN 1 ELSE 0 END) AS BrojKasnihZadataka
                FROM Korisnici k
                LEFT JOIN Projekti p ON k.KorisnikId = p.KreiraoKorisnikId
                LEFT JOIN Zadaci zk ON k.KorisnikId = zk.KreiraoKorisnikId
                LEFT JOIN Zadaci zd ON k.KorisnikId = zd.DodeljenKorisnikId
                WHERE k.JeAktivan = 1
                GROUP BY k.KorisnikId, k.KorisnickoIme, k.Ime, k.Prezime, k.Email;
            ");

            // View: Project Statistics Overview
            migrationBuilder.Sql(@"
                CREATE VIEW ProjektiStatistikaView AS
                SELECT 
                    p.Kategorija,
                    COUNT(DISTINCT p.ProjekatId) AS BrojProjеkata,
                    SUM(CASE WHEN p.Status = 'Aktivan' THEN 1 ELSE 0 END) AS AktivniProjekti,
                    SUM(CASE WHEN p.Status = 'Zavrsen' THEN 1 ELSE 0 END) AS ZavrseniProjekti,
                    SUM(CASE WHEN p.Status = 'NaCekanju' THEN 1 ELSE 0 END) AS ProjektiNaCekanju,
                    COUNT(z.ZadatakId) AS UkupnoZadataka,
                    ROUND(AVG(
                        (SELECT COUNT(*) FROM Zadaci WHERE ProjekatId = p.ProjekatId)
                    ), 2) AS ProsecnoZadatakaPoProjektu,
                    ROUND(
                        SUM(CASE WHEN z.Status = 'Zavrsen' THEN 1 ELSE 0 END) * 100.0 / 
                        NULLIF(COUNT(z.ZadatakId), 0), 2
                    ) AS ProcenatZavrsenihZadataka
                FROM Projekti p
                LEFT JOIN Zadaci z ON p.ProjekatId = z.ProjekatId
                GROUP BY p.Kategorija;
            ");

            // View: Overdue Tasks
            migrationBuilder.Sql(@"
                CREATE VIEW ZadaciKasneView AS
                SELECT 
                    z.ZadatakId,
                    z.Naslov AS ZadatakNaslov,
                    z.Status,
                    z.Prioritet,
                    z.RokIzvrsenja,
                    DATEDIFF(UTC_TIMESTAMP(), z.RokIzvrsenja) AS DanaKasni,
                    p.ProjekatId,
                    p.Naziv AS ProjekatNaziv,
                    p.Kategorija AS ProjekatKategorija,
                    k.KorisnikId AS DodeljenKorisnikId,
                    k.KorisnickoIme AS DodeljenKorisnickoIme,
                    CONCAT(k.Ime, ' ', k.Prezime) AS DodeljenPunoIme
                FROM Zadaci z
                INNER JOIN Projekti p ON z.ProjekatId = p.ProjekatId
                LEFT JOIN Korisnici k ON z.DodeljenKorisnikId = k.KorisnikId
                WHERE z.RokIzvrsenja < UTC_TIMESTAMP() 
                  AND z.Status NOT IN ('Zavrsen')
                ORDER BY z.RokIzvrsenja ASC;
            ");

            // View: Task Distribution by Status and Priority
            migrationBuilder.Sql(@"
                CREATE VIEW ZadaciDistribucijaView AS
                SELECT 
                    z.Status,
                    z.Prioritet,
                    COUNT(*) AS BrojZadataka,
                    ROUND(COUNT(*) * 100.0 / (SELECT COUNT(*) FROM Zadaci), 2) AS Procenat,
                    AVG(CASE 
                        WHEN z.RokIzvrsenja IS NOT NULL 
                        THEN DATEDIFF(z.RokIzvrsenja, UTC_TIMESTAMP()) 
                        ELSE NULL 
                    END) AS ProsecnoDanaDoRoka
                FROM Zadaci z
                GROUP BY z.Status, z.Prioritet
                ORDER BY 
                    CASE z.Status
                        WHEN 'Blokiran' THEN 1
                        WHEN 'UraduJe' THEN 2
                        WHEN 'NaCekanju' THEN 3
                        WHEN 'Zavrsen' THEN 4
                        ELSE 5
                    END,
                    CASE z.Prioritet
                        WHEN 'Hitan' THEN 1
                        WHEN 'Visok' THEN 2
                        WHEN 'Srednji' THEN 3
                        WHEN 'Nizak' THEN 4
                        ELSE 5
                    END;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop Views
            migrationBuilder.Sql("DROP VIEW IF EXISTS ZadaciDistribucijaView;");
            migrationBuilder.Sql("DROP VIEW IF EXISTS ZadaciKasneView;");
            migrationBuilder.Sql("DROP VIEW IF EXISTS ProjektiStatistikaView;");
            migrationBuilder.Sql("DROP VIEW IF EXISTS KorisnikStatistikaView;");
            migrationBuilder.Sql("DROP VIEW IF EXISTS AktivniProjektiSaZadacimaView;");

            // Drop Zadatak Stored Procedures
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_PreuzmiZadatkeNaCekanju;");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_PreuzmiZadatkePoKorisniku;");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_PreuzmiZadatkePoProjektu;");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_ObrisiZadatak;");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_AzurirajZadatak;");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_PreuzmiSveZadatke;");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_PreuzmiZadatakPoId;");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_KreirajZadatak;");

            // Drop Projekat Stored Procedures
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_PreuzmiProjekatSaZadacima;");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_PreuzmiProjektePoKorisniku;");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_PreuzmiAktivneProjekte;");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_ObrisiProjekat;");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_AzurirajProjekat;");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_PreuzmiSveProjekte;");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_PreuzmiProjekatPoId;");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_KreirajProjekat;");

            // Drop Korisnik Stored Procedures
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_PronadjiKorisnikaPoEmailu;");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_PronadjiKorisnikaPoKorisnickomImenu;");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_ObrisiKorisnika;");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_AzurirajKorisnika;");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_PreuzmiSveKorisnike;");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_PreuzmiKorisnikaPoId;");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_KreirajKorisnika;");
        }
    }
}
