using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using CreativeAgency.Web.ViewModels;
using CreativeAgency.Business;

namespace CreativeAgency.Web.Controllers
{
    [Route("Nalog/[action]")]
    public class NalogController : Controller
    {
        private readonly KorisnikServis _korisnikServis;

        public NalogController(KorisnikServis korisnikServis)
        {
            _korisnikServis = korisnikServis;
        }

        [HttpGet]
        public IActionResult Prijava(string? povratnaStranica = null)
        {
            ViewData["PovratnaStranica"] = povratnaStranica;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Prijava(PrijavaViewModel model, string? povratnaStranica = null)
        {
            ViewData["PovratnaStranica"] = povratnaStranica;

            if (!ModelState.IsValid)
                return View(model);

            var korisnik = await _korisnikServis.AutentifikujAsync(model.KorisnickoIme, model.Lozinka);
            
            if (korisnik == null)
            {
                ModelState.AddModelError(string.Empty, "Nevažeće korisničko ime ili lozinka");
                return View(model);
            }

            var tvrdnje = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, korisnik.KorisnikId.ToString()),
                new Claim(ClaimTypes.Name, korisnik.KorisnickoIme),
                new Claim(ClaimTypes.Email, korisnik.Email)
            };

            var identitetTvrdnji = new ClaimsIdentity(tvrdnje, CookieAuthenticationDefaults.AuthenticationScheme);
            var osobineAutentifikacije = new AuthenticationProperties
            {
                IsPersistent = model.ZapamtiMe,
                ExpiresUtc = model.ZapamtiMe ? DateTimeOffset.UtcNow.AddDays(30) : DateTimeOffset.UtcNow.AddHours(24)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identitetTvrdnji),
                osobineAutentifikacije);

            if (!string.IsNullOrEmpty(povratnaStranica) && Url.IsLocalUrl(povratnaStranica))
                return Redirect(povratnaStranica);

            return RedirectToAction("Pocetna", "Pocetna");
        }

        [HttpGet]
        public IActionResult Registracija()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registracija(RegistracijaViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var korisnik = await _korisnikServis.RegistrujAsync(
                    model.KorisnickoIme,
                    model.Email,
                    model.Lozinka,
                    model.Ime,
                    model.Prezime
                );

                TempData["PorukaUspeha"] = "Registracija uspešna! Molimo prijavite se.";
                return RedirectToAction(nameof(Prijava));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Odjava()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Pocetna", "Pocetna");
        }

        public IActionResult PristupOdbijen()
        {
            return View();
        }
    }
}
