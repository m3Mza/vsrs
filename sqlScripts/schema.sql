-- ============================================
-- Creative Agency Database - Schema Definition
-- ============================================
-- Database: creativeagency_db
-- Description: Tables, views, indexes, and constraints
-- ============================================

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET time_zone = "+00:00";

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

-- ============================================
-- TABLES
-- ============================================

-- Table: Korisnici (Users)
CREATE TABLE IF NOT EXISTS `Korisnici` (
    `KorisnikId` INT NOT NULL AUTO_INCREMENT,
    `KorisnickoIme` VARCHAR(100) NOT NULL,
    `Email` VARCHAR(255) NOT NULL,
    `LozinkaHash` VARCHAR(255) NOT NULL,
    `Ime` VARCHAR(100) DEFAULT NULL,
    `Prezime` VARCHAR(100) DEFAULT NULL,
    `DatumKreiranja` DATETIME(6) NOT NULL,
    `DatumPoslednjegPrijave` DATETIME(6) DEFAULT NULL,
    `JeAktivan` TINYINT(1) NOT NULL DEFAULT 1,
    PRIMARY KEY (`KorisnikId`),
    UNIQUE KEY `IX_Korisnici_Email` (`Email`),
    UNIQUE KEY `IX_Korisnici_KorisnickoIme` (`KorisnickoIme`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Table: Projekti (Projects)
CREATE TABLE IF NOT EXISTS `Projekti` (
    `ProjekatId` INT NOT NULL AUTO_INCREMENT,
    `Naziv` VARCHAR(200) NOT NULL,
    `Opis` VARCHAR(1000) DEFAULT NULL,
    `Status` VARCHAR(50) NOT NULL,
    `Kategorija` VARCHAR(50) NOT NULL DEFAULT 'Sajt',
    `DatumPocetka` DATETIME(6) NOT NULL,
    `DatumZavrsetka` DATETIME(6) DEFAULT NULL,
    `DatumKreiranja` DATETIME(6) NOT NULL,
    `DatumAzuriranja` DATETIME(6) DEFAULT NULL,
    `KreiraoKorisnikId` INT DEFAULT NULL,
    PRIMARY KEY (`ProjekatId`),
    KEY `IX_Projekti_KreiraoKorisnikId` (`KreiraoKorisnikId`),
    CONSTRAINT `FK_Projekti_Korisnici_KreiraoKorisnikId` 
        FOREIGN KEY (`KreiraoKorisnikId`) 
        REFERENCES `Korisnici` (`KorisnikId`) 
        ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Table: Zadaci (Tasks)
CREATE TABLE IF NOT EXISTS `Zadaci` (
    `ZadatakId` INT NOT NULL AUTO_INCREMENT,
    `ProjekatId` INT NOT NULL,
    `Naslov` VARCHAR(200) NOT NULL,
    `Opis` VARCHAR(1000) DEFAULT NULL,
    `Status` VARCHAR(50) NOT NULL,
    `Prioritet` VARCHAR(50) NOT NULL,
    `RokIzvrsenja` DATETIME(6) DEFAULT NULL,
    `DatumKreiranja` DATETIME(6) NOT NULL,
    `DatumAzuriranja` DATETIME(6) DEFAULT NULL,
    `DodeljenKorisnikId` INT DEFAULT NULL,
    `KreiraoKorisnikId` INT DEFAULT NULL,
    PRIMARY KEY (`ZadatakId`),
    KEY `IX_Zadaci_ProjekatId` (`ProjekatId`),
    KEY `IX_Zadaci_DodeljenKorisnikId` (`DodeljenKorisnikId`),
    KEY `IX_Zadaci_KreiraoKorisnikId` (`KreiraoKorisnikId`),
    CONSTRAINT `FK_Zadaci_Projekti_ProjekatId` 
        FOREIGN KEY (`ProjekatId`) 
        REFERENCES `Projekti` (`ProjekatId`) 
        ON DELETE CASCADE,
    CONSTRAINT `FK_Zadaci_Korisnici_DodeljenKorisnikId` 
        FOREIGN KEY (`DodeljenKorisnikId`) 
        REFERENCES `Korisnici` (`KorisnikId`) 
        ON DELETE SET NULL,
    CONSTRAINT `FK_Zadaci_Korisnici_KreiraoKorisnikId` 
        FOREIGN KEY (`KreiraoKorisnikId`) 
        REFERENCES `Korisnici` (`KorisnikId`) 
        ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Table: __EFMigrationsHistory (Entity Framework Migrations)
CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
    `MigrationId` VARCHAR(150) NOT NULL,
    `ProductVersion` VARCHAR(32) NOT NULL,
    PRIMARY KEY (`MigrationId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- ============================================
-- VIEWS
-- ============================================

-- View: AktivniProjektiSaZadacimaView
-- Description: Shows active projects with task statistics
DROP VIEW IF EXISTS `AktivniProjektiSaZadacimaView`;

CREATE VIEW `AktivniProjektiSaZadacimaView` AS
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
    ROUND((SUM(CASE WHEN z.Status = 'Zavrsen' THEN 1 ELSE 0 END) * 100.0) / NULLIF(COUNT(z.ZadatakId), 0), 2) AS ProcenatZavrsenosti
FROM Projekti p
LEFT JOIN Korisnici k ON p.KreiraoKorisnikId = k.KorisnikId
LEFT JOIN Zadaci z ON p.ProjekatId = z.ProjekatId
WHERE p.Status = 'Aktivan'
GROUP BY p.ProjekatId, p.Naziv, p.Kategorija, p.DatumPocetka, p.DatumZavrsetka, 
         k.KorisnickoIme, k.Ime, k.Prezime;

-- View: KorisnikStatistikaView
-- Description: User statistics including projects and tasks
DROP VIEW IF EXISTS `KorisnikStatistikaView`;

CREATE VIEW `KorisnikStatistikaView` AS
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
    SUM(CASE WHEN zd.RokIzvrsenja < UTC_TIMESTAMP() AND zd.Status <> 'Zavrsen' THEN 1 ELSE 0 END) AS BrojKasnihZadataka
FROM Korisnici k
LEFT JOIN Projekti p ON k.KorisnikId = p.KreiraoKorisnikId
LEFT JOIN Zadaci zk ON k.KorisnikId = zk.KreiraoKorisnikId
LEFT JOIN Zadaci zd ON k.KorisnikId = zd.DodeljenKorisnikId
WHERE k.JeAktivan = 1
GROUP BY k.KorisnikId, k.KorisnickoIme, k.Ime, k.Prezime, k.Email;

-- View: ProjektiStatistikaView
-- Description: Project statistics grouped by category
DROP VIEW IF EXISTS `ProjektiStatistikaView`;

CREATE VIEW `ProjektiStatistikaView` AS
SELECT 
    p.Kategorija,
    COUNT(DISTINCT p.ProjekatId) AS BrojProjеkata,
    SUM(CASE WHEN p.Status = 'Aktivan' THEN 1 ELSE 0 END) AS AktivniProjekti,
    SUM(CASE WHEN p.Status = 'Zavrsen' THEN 1 ELSE 0 END) AS ZavrseniProjekti,
    SUM(CASE WHEN p.Status = 'NaCekanju' THEN 1 ELSE 0 END) AS ProjektiNaCekanju,
    COUNT(z.ZadatakId) AS UkupnoZadataka,
    ROUND(AVG((SELECT COUNT(*) FROM Zadaci WHERE Zadaci.ProjekatId = p.ProjekatId)), 2) AS ProsecnoZadatakaPoProjektu,
    ROUND((SUM(CASE WHEN z.Status = 'Zavrsen' THEN 1 ELSE 0 END) * 100.0) / NULLIF(COUNT(z.ZadatakId), 0), 2) AS ProcenatZavrsenihZadataka
FROM Projekti p
LEFT JOIN Zadaci z ON p.ProjekatId = z.ProjekatId
GROUP BY p.Kategorija;

-- View: ZadaciKasneView
-- Description: Overdue tasks with details
DROP VIEW IF EXISTS `ZadaciKasneView`;

CREATE VIEW `ZadaciKasneView` AS
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
  AND z.Status <> 'Zavrsen'
ORDER BY z.RokIzvrsenja ASC;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
