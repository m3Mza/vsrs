Seminarski rad iz višeslojnog razvoja softvera

Delovi:

1. ASP.NET aplikacija
2. MySQL baza podataka

Za pokretanje

1. Potreban Docker za pokretanje lokalnog servera, nakon kreacije kontejnera sa lokalnim serverom pokretanje servera: 

docker-compose up -d

2. Za pokretanje aplikacije

cd CreativeAgency.Web
dotnet run


sajt na 5500 port
baza na 8081 port



Ispravke nakon odbrane seminarskog rada, 3.4.2026

Zahtevi: 

1. CreativeAgency.Business, CreativeAgency.DAL, CreativeAgency.Services, CreativeAgency.Web - nije u skladu sa strukturom seminarskog rada, 2. Ne moze biti kod na engleskom. 3. DBUtils se ne nalazi u projektu,  4. prezentaciona logika nije prisutna u projektu, 5. Ne postoji segment Klasa Podataka u Sloju Podataka, 6. Ne postoji SOAP veb servis u projektu/XML.



1. kod za soap veb servis se nalazi u CreativeAgency.Services folderu

2. prezentaciona logika se nalazi u CreativeAgency.Web/Controllers (kontroleri vrse logiku obrade zahteva i komuniciraju sa posslovnim slojem) a ViewModeli se nalaze u CreativeAgency.Web/ViewModel i sluze za prikaz podataka na cshtml stranicama

3. klase podataka se nalaze u CreativeAgency.Models

4. u word dokumentu dodati aktuelni nazivi slojeva u projektu, odvojen deo koda za DAL i repozitorijumski sloj

5. dodat dbutils u CreativeAgency.DAL

6. Metode, klase, objekti i sl. su vec na srpskom, preveo nazive fajlova i foldera za slojeve na srpski