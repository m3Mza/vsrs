using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CreativeAgency.Web.ViewModels;
using CreativeAgency.Business;
using CreativeAgency.Models;
using System.Security.Claims;

namespace CreativeAgency.Web.Controllers
{
    [Route("Projekti/[action]")]
    public class ProjektiController : Controller
    {
        private readonly ProjekatServis _projekatServis;
        private readonly ZadatakServis _zadatakServis;
        private readonly KorisnikServis _korisnikServis;

        public ProjektiController(
            ProjekatServis projekatServis,
            ZadatakServis zadatakServis,
            KorisnikServis korisnikServis)
        {
            _projekatServis = projekatServis;
            _zadatakServis = zadatakServis;
            _korisnikServis = korisnikServis;
        }

        // GET: Projekti
        public async Task<IActionResult> Pregled()
        {
            var projekti = User.Identity?.IsAuthenticated ?? false
                ? await _projekatServis.PreuzmiAktivneProjekteAsync()
                : (await _projekatServis.PreuzmiSveProjekteAsync()).Where(p => p.Status == "Zavrsen");
            
            var viewModel = new ListaProjekataViewModel
            {
                Projekti = projekti,
                MozeUredjivati = User.Identity?.IsAuthenticated ?? false,
                UkupnoProjekta = projekti.Count()
            };

            return View(viewModel);
        }

        // GET: Projekti/Detalji/5
        public async Task<IActionResult> Detalji(int id)
        {
            var projekat = await _projekatServis.PreuzmiProjekatSaZadacimaAsync(id);
            if (projekat == null)
                return NotFound();

            var korisnikId = User.Identity?.IsAuthenticated == true 
                ? int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!) 
                : (int?)null;

            var viewModel = new DetaljiProjektaViewModel
            {
                Projekat = projekat,
                Zadaci = projekat.Zadaci,
                MozeUredjivati = korisnikId.HasValue && projekat.KreiraoKorisnikId == korisnikId.Value
            };

            return View(viewModel);
        }

        // GET: Projekti/Kreiraj
        [Authorize]
        public IActionResult Kreiraj()
        {
            return View();
        }

        // POST: Projekti/Kreiraj
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Kreiraj(KreirajProjekatViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var korisnikId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var projekat = new Projekat
            {
                Naziv = model.Naziv,
                Opis = model.Opis,
                Status = "Aktivan",
                Kategorija = model.Kategorija,
                DatumPocetka = model.DatumPocetka,
                DatumZavrsetka = null,
                DatumKreiranja = DateTime.UtcNow,
                KreiraoKorisnikId = korisnikId
            };

            await _projekatServis.DodajProjekatAsync(projekat);

            TempData["PorukaUspeha"] = "Projekat uspešno kreiran!";
            return RedirectToAction(nameof(Pregled));
        }

        // GET: Projekti/Uredi/5
        [Authorize]
        public async Task<IActionResult> Uredi(int id)
        {
            var projekat = await _projekatServis.PreuzmiProjekatPoIdAsync(id);
            if (projekat == null)
                return NotFound();

            var korisnikId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            if (projekat.KreiraoKorisnikId != korisnikId)
            {
                TempData["PorukaGreske"] = "Nemate dozvolu da uređujete ovaj projekat.";
                return RedirectToAction(nameof(Detalji), new { id });
            }

            var viewModel = new KreirajProjekatViewModel
            {
                Naziv = projekat.Naziv,
                Opis = projekat.Opis,
                Status = projekat.Status,
                Kategorija = projekat.Kategorija,
                DatumPocetka = projekat.DatumPocetka
            };

            ViewData["ProjekatId"] = id;
            return View(viewModel);
        }

        // POST: Projekti/Uredi/5
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Uredi(int id, KreirajProjekatViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var projekat = await _projekatServis.PreuzmiProjekatPoIdAsync(id);
            if (projekat == null)
                return NotFound();

            var korisnikId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            if (projekat.KreiraoKorisnikId != korisnikId)
            {
                TempData["PorukaGreske"] = "Nemate dozvolu da uređujete ovaj projekat.";
                return RedirectToAction(nameof(Detalji), new { id });
            }

            projekat.Naziv = model.Naziv;
            projekat.Opis = model.Opis;
            projekat.Kategorija = model.Kategorija;
            projekat.DatumPocetka = model.DatumPocetka;
            
            // Automatically set finish date when status changes to Zavrsen
            if (model.Status == "Zavrsen" && projekat.Status != "Zavrsen")
            {
                projekat.DatumZavrsetka = DateTime.UtcNow;
            }
            projekat.Status = model.Status;

            await _projekatServis.AzurirajProjekatAsync(projekat);

            TempData["PorukaUspeha"] = "Projekat uspešno ažuriran!";
            return RedirectToAction(nameof(Detalji), new { id });
        }

        // POST: Projekti/Obrisi/5
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Obrisi(int id)
        {
            var projekat = await _projekatServis.PreuzmiProjekatPoIdAsync(id);
            if (projekat == null)
                return NotFound();

            var korisnikId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            if (projekat.KreiraoKorisnikId != korisnikId)
            {
                TempData["PorukaGreske"] = "Nemate dozvolu da obrišete ovaj projekat.";
                return RedirectToAction(nameof(Detalji), new { id });
            }

            await _projekatServis.ObrisiProjekatAsync(id);
            TempData["PorukaUspeha"] = "Projekat uspešno obrisan!";
            return RedirectToAction(nameof(Pregled));
        }
    }
}
