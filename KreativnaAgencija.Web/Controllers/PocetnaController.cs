using Microsoft.AspNetCore.Mvc;
using CreativeAgency.Web.ViewModels;
using CreativeAgency.Business;

namespace CreativeAgency.Web.Controllers
{
    public class PocetnaController : Controller
    {
        private readonly ProjekatServis _projekatServis;
        private readonly ZadatakServis _zadatakServis;

        public PocetnaController(ProjekatServis projekatServis, ZadatakServis zadatakServis)
        {
            _projekatServis = projekatServis;
            _zadatakServis = zadatakServis;
        }

        [Route("")]
        [Route("Pocetna")]
        [Route("Pocetna/Pocetna")]
        public async Task<IActionResult> Pocetna()
        {
            var viewModel = new PocetnaViewModel
            {
                UkupnoProjekta = await _projekatServis.PrebrојProjekteAsync(),
                AktivnihProjekta = (await _projekatServis.PreuzmiAktivneProjekteAsync()).Count(),
                UkupnoZadataka = (await _zadatakServis.PreuzmiSveZadatkeAsync()).Count(),
                ZadatakaNaCekanju = (await _zadatakServis.PreuzmiZadatkeNaCekanjuAsync()).Count(),
                JeAutentifikovan = User.Identity?.IsAuthenticated ?? false
            };

            return View(viewModel);
        }

        [Route("Pocetna/Privatnost")]
        public IActionResult Privatnost()
        {
            return View();
        }

        [Route("Pocetna/Greska")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Greska()
        {
            ViewData["IdZahteva"] = HttpContext.TraceIdentifier;
            ViewData["PrikaziIdZahteva"] = !string.IsNullOrEmpty(HttpContext.TraceIdentifier);
            return View();
        }
    }
}
