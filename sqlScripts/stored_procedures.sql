-- ============================================
-- Creative Agency Database - Stored Procedures
-- ============================================
-- Database: creativeagency_db
-- Description: All stored procedures for CRUD operations
-- ============================================

USE creativeagency_db;

DELIMITER $$

-- ============================================
-- KORISNICI (Users) Procedures
-- ============================================

-- Create new user
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
END$$

-- Update user
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
END$$

-- Delete user
CREATE PROCEDURE sp_ObrisiKorisnika(
    IN p_KorisnikId INT
)
BEGIN
    DELETE FROM Korisnici WHERE KorisnikId = p_KorisnikId;
END$$

-- Get all users
CREATE PROCEDURE sp_PreuzmiSveKorisnike()
BEGIN
    SELECT * FROM Korisnici ORDER BY DatumKreiranja DESC;
END$$

-- Get user by ID
CREATE PROCEDURE sp_PreuzmiKorisnikaPoId(
    IN p_KorisnikId INT
)
BEGIN
    SELECT * FROM Korisnici WHERE KorisnikId = p_KorisnikId;
END$$

-- Find user by username
CREATE PROCEDURE sp_PronadjiKorisnikaPoKorisnickomImenu(
    IN p_KorisnickoIme VARCHAR(100)
)
BEGIN
    SELECT * FROM Korisnici WHERE KorisnickoIme = p_KorisnickoIme LIMIT 1;
END$$

-- Find user by email
CREATE PROCEDURE sp_PronadjiKorisnikaPoEmailu(
    IN p_Email VARCHAR(255)
)
BEGIN
    SELECT * FROM Korisnici WHERE Email = p_Email LIMIT 1;
END$$

-- ============================================
-- PROJEKTI (Projects) Procedures
-- ============================================

-- Create new project
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
END$$

-- Update project
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
END$$

-- Delete project
CREATE PROCEDURE sp_ObrisiProjekat(
    IN p_ProjekatId INT
)
BEGIN
    DELETE FROM Projekti WHERE ProjekatId = p_ProjekatId;
END$$

-- Get all projects
CREATE PROCEDURE sp_PreuzmiSveProjekte()
BEGIN
    SELECT * FROM Projekti ORDER BY DatumKreiranja DESC;
END$$

-- Get project by ID
CREATE PROCEDURE sp_PreuzmiProjekatPoId(
    IN p_ProjekatId INT
)
BEGIN
    SELECT * FROM Projekti WHERE ProjekatId = p_ProjekatId;
END$$

-- Get active projects with creator info
CREATE PROCEDURE sp_PreuzmiAktivneProjekte()
BEGIN
    SELECT p.*, k.KorisnickoIme, k.Ime, k.Prezime
    FROM Projekti p
    LEFT JOIN Korisnici k ON p.KreiraoKorisnikId = k.KorisnikId
    WHERE p.Status = 'Aktivan'
    ORDER BY p.DatumKreiranja DESC;
END$$

-- Get projects by user
CREATE PROCEDURE sp_PreuzmiProjektePoKorisniku(
    IN p_KorisnikId INT
)
BEGIN
    SELECT p.*, 
           (SELECT COUNT(*) FROM Zadaci z WHERE z.ProjekatId = p.ProjekatId) AS BrojZadataka
    FROM Projekti p
    WHERE p.KreiraoKorisnikId = p_KorisnikId
    ORDER BY p.DatumKreiranja DESC;
END$$

-- Get project with all tasks
CREATE PROCEDURE sp_PreuzmiProjekatSaZadacima(
    IN p_ProjekatId INT
)
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
END$$

-- ============================================
-- ZADACI (Tasks) Procedures
-- ============================================

-- Create new task
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
END$$

-- Update task
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
END$$

-- Delete task
CREATE PROCEDURE sp_ObrisiZadatak(
    IN p_ZadatakId INT
)
BEGIN
    DELETE FROM Zadaci WHERE ZadatakId = p_ZadatakId;
END$$

-- Get all tasks with details
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
END$$

-- Get task by ID
CREATE PROCEDURE sp_PreuzmiZadatakPoId(
    IN p_ZadatakId INT
)
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
END$$

-- Get tasks by project
CREATE PROCEDURE sp_PreuzmiZadatkePoProjektu(
    IN p_ProjekatId INT
)
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
END$$

-- Get tasks by user (assigned to)
CREATE PROCEDURE sp_PreuzmiZadatkePoKorisniku(
    IN p_KorisnikId INT
)
BEGIN
    SELECT z.*, 
           p.Naziv AS ProjekatNaziv, p.Status AS ProjekatStatus
    FROM Zadaci z
    LEFT JOIN Projekti p ON z.ProjekatId = p.ProjekatId
    WHERE z.DodeljenKorisnikId = p_KorisnikId
    ORDER BY z.RokIzvrsenja ASC;
END$$

-- Get pending tasks
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
END$$

DELIMITER ;
