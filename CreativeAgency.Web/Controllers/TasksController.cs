using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CreativeAgency.Web.ViewModels;
using CreativeAgency.Business;
using System.Security.Claims;

namespace CreativeAgency.Web.Controllers
{
    [Route("Zadaci/[action]")]
    [Authorize]
    public class ZadaciController : Controller
    {
        private readonly ZadatakServis _zadatakServis;
        private readonly ProjekatServis _projekatServis;
        private readonly KorisnikServis _korisnikServis;

        public ZadaciController(
            ZadatakServis zadatakServis,
            ProjekatServis projekatServis,
            KorisnikServis korisnikServis)
        {
            _zadatakServis = zadatakServis;
            _projekatServis = projekatServis;
            _korisnikServis = korisnikServis;
        }

        // GET: Zadaci
        public async Task<IActionResult> Pregled(int? projekatId)
        {
            IEnumerable<CreativeAgency.Models.Zadatak> zadaci;
            string? nazivProjekta = null;

            if (projekatId.HasValue)
            {
                zadaci = await _zadatakServis.PreuzmiZadatkePoProjetkuAsync(projekatId.Value);
                var projekat = await _projekatServis.PreuzmiProjekatPoIdAsync(projekatId.Value);
                nazivProjekta = projekat?.Naziv;
            }
            else
            {
                zadaci = await _zadatakServis.PreuzmiSveZadatkeAsync();
            }

            var viewModel = new ListaZadatakaViewModel
            {
                Zadaci = zadaci,
                ProjekatId = projekatId,
                NazivProjekta = nazivProjekta
            };

            return View(viewModel);
        }

        // GET: Zadaci/Kreiraj
        public async Task<IActionResult> Kreiraj(int? projekatId)
        {
            ViewBag.Korisnici = await _korisnikServis.PreuzmiSveKorisnikeAsync();
            
            if (projekatId.HasValue)
            {
                ViewBag.Projekti = new[] { await _projekatServis.PreuzmiProjekatPoIdAsync(projekatId.Value) };
                return View(new KreirajZadatakViewModel { ProjekatId = projekatId.Value });
            }

            ViewBag.Projekti = await _projekatServis.PreuzmiSveProjekteAsync();
            return View(new KreirajZadatakViewModel());
        }

        // POST: Zadaci/Kreiraj
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Kreiraj(KreirajZadatakViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Korisnici = await _korisnikServis.PreuzmiSveKorisnikeAsync();
                ViewBag.Projekti = await _projekatServis.PreuzmiSveProjekteAsync();
                return View(model);
            }

            var korisnikId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            try
            {
                await _zadatakServis.KreirajZadatakAsync(
                    model.ProjekatId,
                    model.Naslov,
                    model.Opis,
                    model.Prioritet,
                    model.RokIzvrsenja,
                    model.DodeljenKorisnikId,
                    korisnikId
                );

                TempData["PorukaUspeha"] = "Zadatak uspešno kreiran!";
                return RedirectToAction("Detalji", "Projekti", new { id = model.ProjekatId });
            }
            catch (InvalidOperationException ex)
            {
                TempData["PorukaGreske"] = ex.Message;
                ViewBag.Korisnici = await _korisnikServis.PreuzmiSveKorisnikeAsync();
                ViewBag.Projekti = await _projekatServis.PreuzmiSveProjekteAsync();
                return View(model);
            }
        }

        // GET: Zadaci/Uredi/5
        public async Task<IActionResult> Uredi(int id)
        {
            var zadatak = await _zadatakServis.PreuzmiZadatakPoIdAsync(id);
            if (zadatak == null)
                return NotFound();

            var korisnikId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var projekat = await _projekatServis.PreuzmiProjekatPoIdAsync(zadatak.ProjekatId);
            
            // Check if user is project creator OR assigned to this task
            bool canEdit = projekat?.KreiraoKorisnikId == korisnikId || zadatak.DodeljenKorisnikId == korisnikId;
            if (!canEdit)
            {
                TempData["PorukaGreske"] = "Nemate dozvolu da uređujete ovaj zadatak.";
                return RedirectToAction("Detalji", "Projekti", new { id = zadatak.ProjekatId });
            }

            ViewBag.Korisnici = await _korisnikServis.PreuzmiSveKorisnikeAsync();
            ViewBag.Projekti = await _projekatServis.PreuzmiSveProjekteAsync();
            ViewBag.Statusi = new[] { "NaCekanju", "UObradi", "Zavrseno", "Blokirano" };

            var viewModel = new KreirajZadatakViewModel
            {
                ProjekatId = zadatak.ProjekatId,
                Naslov = zadatak.Naslov,
                Opis = zadatak.Opis,
                Prioritet = zadatak.Prioritet,
                RokIzvrsenja = zadatak.RokIzvrsenja,
                DodeljenKorisnikId = zadatak.DodeljenKorisnikId
            };

            ViewData["ZadatakId"] = id;
            ViewData["Status"] = zadatak.Status;
            ViewData["IsAssignedUser"] = zadatak.DodeljenKorisnikId == korisnikId;
            return View(viewModel);
        }

        // POST: Zadaci/Uredi/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Uredi(int id, KreirajZadatakViewModel model, string status)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Korisnici = await _korisnikServis.PreuzmiSveKorisnikeAsync();
                ViewBag.Projekti = await _projekatServis.PreuzmiSveProjekteAsync();
                ViewBag.Statusi = new[] { "NaCekanju", "UObradi", "Zavrseno", "Blokirano" };
                return View(model);
            }

            var zadatak = await _zadatakServis.PreuzmiZadatakPoIdAsync(id);
            if (zadatak == null)
                return NotFound();

            var korisnikId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var projekat = await _projekatServis.PreuzmiProjekatPoIdAsync(zadatak.ProjekatId);
            
            // Check if user is project creator OR assigned to this task
            bool canEdit = projekat?.KreiraoKorisnikId == korisnikId || zadatak.DodeljenKorisnikId == korisnikId;
            if (!canEdit)
            {
                TempData["PorukaGreske"] = "Nemate dozvolu da uređujete ovaj zadatak.";
                return RedirectToAction("Detalji", "Projekti", new { id = zadatak.ProjekatId });
            }

            zadatak.Naslov = model.Naslov;
            zadatak.Opis = model.Opis;
            zadatak.Prioritet = model.Prioritet;
            zadatak.RokIzvrsenja = model.RokIzvrsenja;
            zadatak.Status = status;

            // Check if user assignment is changing
            if (zadatak.DodeljenKorisnikId != model.DodeljenKorisnikId && model.DodeljenKorisnikId.HasValue)
            {
                try
                {
                    // Validate business rule before updating
                    await _zadatakServis.DodeliZadatakAsync(id, model.DodeljenKorisnikId.Value);
                }
                catch (InvalidOperationException ex)
                {
                    TempData["PorukaGreske"] = ex.Message;
                    ViewBag.Korisnici = await _korisnikServis.PreuzmiSveKorisnikeAsync();
                    ViewBag.Projekti = await _projekatServis.PreuzmiSveProjekteAsync();
                    ViewBag.Statusi = new[] { "NaCekanju", "UObradi", "Zavrseno", "Blokirano" };
                    ViewData["ZadatakId"] = id;
                    ViewData["Status"] = status;
                    return View(model);
                }
            }
            else
            {
                zadatak.DodeljenKorisnikId = model.DodeljenKorisnikId;
                await _zadatakServis.AzurirajZadatakAsync(zadatak);
            }

            TempData["PorukaUspeha"] = "Zadatak uspešno ažuriran!";
            return RedirectToAction("Detalji", "Projekti", new { id = zadatak.ProjekatId });
        }

        // POST: Zadaci/Obrisi/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Obrisi(int id)
        {
            var zadatak = await _zadatakServis.PreuzmiZadatakPoIdAsync(id);
            if (zadatak == null)
                return NotFound();

            var korisnikId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var projekat = await _projekatServis.PreuzmiProjekatPoIdAsync(zadatak.ProjekatId);
            
            // Only project creator can delete tasks
            if (projekat?.KreiraoKorisnikId != korisnikId)
            {
                TempData["PorukaGreske"] = "Samo kreator projekta može obrisati zadatke.";
                return RedirectToAction("Detalji", "Projekti", new { id = zadatak.ProjekatId });
            }

            var projekatId = zadatak.ProjekatId;
            await _zadatakServis.ObrisiZadatakAsync(id);

            TempData["PorukaUspeha"] = "Zadatak uspešno obrisan!";
            return RedirectToAction("Detalji", "Projekti", new { id = projekatId });
        }

        // POST: Zadaci/AzurirajStatus/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AzurirajStatus(int id, string status)
        {
            await _zadatakServis.AzurirajStatusZadatkaAsync(id, status);
            return RedirectToAction(nameof(Pregled));
        }
    }
}
