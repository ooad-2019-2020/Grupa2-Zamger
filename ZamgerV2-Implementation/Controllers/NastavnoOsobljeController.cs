using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZamgerV2_Implementation.Helpers;
using ZamgerV2_Implementation.Models;

namespace ZamgerV2_Implementation.Controllers
{
    [Autorizacija(false, TipKorisnika.NastavnoOsoblje, TipKorisnika.Profesor)]
    public class NastavnoOsobljeController : Controller
    {
        private NastavnoOsoblje trenutniKorisnik;
        private ZamgerDbContext zmgr;

        public NastavnoOsobljeController()
        {
            zmgr = ZamgerDbContext.GetInstance();
        }

        [Route("/nastavno-osoblje/dashboard")]
        public IActionResult Dashboard()
        {
            trenutniKorisnik = Autentifikacija.GetNastavnoOsoblje(HttpContext);

            ViewBag.listaObavjestenja = zmgr.dajSvaObavještenja();
            return View(trenutniKorisnik);
        }


        [Route("/nastavno-osoblje/kreiraj-aktivnost")]
        [HttpGet]
        public IActionResult KreirajAktivnost()
        {
            var trenutniKorisnik = Autentifikacija.GetNastavnoOsoblje(HttpContext);
            return View(trenutniKorisnik);
        }

        [Route("/nastavno-osoblje/kreiraj-aktivnost")]
        [HttpPost]
        public IActionResult KreirajAktivnost(int id, IFormCollection forma)
        {
            var trenutniKorisnik = Autentifikacija.GetNastavnoOsoblje(HttpContext);
            DateTime oDate = Convert.ToDateTime(forma["datum"] + " " + forma["vrijeme"]);

            int idAktivnosti = zmgr.kreirajAktivnost(int.Parse(forma["izabraniPredmet"]), forma["naziv"], oDate, forma["vrstaAktivnosti"], double.Parse(forma["maxBrojBodova"]));
            if (forma["vrstaAktivnosti"].Equals("Zadaća"))
            {
                for (int i = 0; i < trenutniKorisnik.PredmetiNaKojimPredaje.Count; i++)
                {
                    if (trenutniKorisnik.PredmetiNaKojimPredaje[i].IdPredmeta == int.Parse(forma["izabraniPredmet"]))
                    {
                        zmgr.ubaciDefaultPodatkeZaZadaću(idAktivnosti, trenutniKorisnik.PredmetiNaKojimPredaje[i], forma["naziv"], double.Parse(forma["maxBrojBodova"]), oDate);
                        break;
                    }
                }
            }
            return RedirectToAction("MojeAktivnosti");
        }


        [Route("/nastavno-osoblje/moje-aktivnosti")]
        public IActionResult MojeAktivnosti()
        {
            var trenutniKorisnik = Autentifikacija.GetNastavnoOsoblje(HttpContext);
            return View(trenutniKorisnik);
        }

        [Route("/nastavno-osoblje/osobe-list")]
        public IActionResult searchUsersForMessage()
        {
            var trenutniKorisnik = Autentifikacija.GetNastavnoOsoblje(HttpContext);
            return View(trenutniKorisnik);
        }

        [Route("/nastavno-osoblje/osobe-list/pretraga")]
        [HttpPost]
        public IActionResult searchUsersForMessageForm(IFormCollection forma)
        {
            var trenutniKorisnik = Autentifikacija.GetNastavnoOsoblje(HttpContext);
            List<Korisnik> korisnici = zmgr.pretražiKorisnike(forma["Ime"], forma["Prezime"]);
            ViewBag.korisnici = korisnici;
            return View(trenutniKorisnik);
        }


        [Route("/nastavno-osoblje/osobe-list/pretraga/{idPrimaoca}")]
        [HttpGet]
        public IActionResult teacherSendMessage(int idPrimaoca)
        {
            var trenutniKorisnik = Autentifikacija.GetNastavnoOsoblje(HttpContext);
            return View(trenutniKorisnik);
        }



        [Route("/nastavno-osoblje/osobe-list/pretraga/{idPrimaoca}")]
        [HttpPost]
        public IActionResult sendMessage(IFormCollection forma, int idPrimaoca)
        {
            var trenutniKorisnik = Autentifikacija.GetNastavnoOsoblje(HttpContext);
            if (forma != null)
            {
                string sadržaj = forma["sadržaj"];
                string naslov = forma["naslov"];
                Poruka poruka = new Poruka(trenutniKorisnik.IdOsobe.Value, idPrimaoca, naslov, sadržaj, DateTime.Now, 0, zmgr.dajNoviPorukaId());
                zmgr.posaljiPoruku(poruka);
                return RedirectToAction("mojOutbox");
            }
            return RedirectToAction("prikaziGresku", new { lokacija = "posalji-poruku", idPoruke = 2 });
        }

        [Route("/nastavno-osoblje/poruke/moj-inbox")]
        public IActionResult mojInbox(int id)
        {
            var trenutniKorisnik = Autentifikacija.GetNastavnoOsoblje(HttpContext);
            return View(trenutniKorisnik);
        }

        [Route("/nastavno-osoblje/poruke/moj-outbox")]
        public IActionResult mojOutbox(int id)
        {
            var trenutniKorisnik = Autentifikacija.GetNastavnoOsoblje(HttpContext);
            return View(trenutniKorisnik);
        }

        [Route("/nastavno-osoblje/poruke/moj-inbox/{idPoruke}")]
        public IActionResult detaljiPorukeInbox(int idPoruke)
        {
            var trenutniKorisnik = Autentifikacija.GetNastavnoOsoblje(HttpContext);
            ViewBag.poruka = zmgr.dajPoruku(idPoruke);
            zmgr.oznaciProcitanu(idPoruke);
            return View(trenutniKorisnik);
        }

        [Route("/nastavno-osoblje/poruke/moj-outbox/{idPoruke}")]
        public IActionResult detaljiPorukeOutbox(int idPoruke)
        {
            var trenutniKorisnik = Autentifikacija.GetNastavnoOsoblje(HttpContext);
            ViewBag.poruka = zmgr.dajPoruku(idPoruke);
            return View(trenutniKorisnik);
        }

        [Route("/nastavno-osoblje/edituj-aktivnost/{idAktivnosti}")]
        [HttpGet]
        public IActionResult editujAktivnost(int idAktivnosti)
        {
            var trenutniKorisnik = Autentifikacija.GetNastavnoOsoblje(HttpContext);
            Aktivnost aktivnost = zmgr.dajAktivnostPoId(idAktivnosti);
            if (aktivnost.GetType() == typeof(Ispit))
            {
                ViewBag.aktivnost = (Ispit)aktivnost;
            }
            else
            {
                ViewBag.aktivnost = (Zadaća)aktivnost;
            }
            return View(trenutniKorisnik);
        }

        [Route("/nastavno-osoblje/edituj-aktivnost/{idAktivnosti}")]
        [HttpPost]
        public IActionResult editujAktivnost(int idAktivnosti, IFormCollection forma)
        {
            if (forma != null)
            {
                DateTime oDate = Convert.ToDateTime(forma["datum"] + " " + forma["vrijeme"]);
                zmgr.editujAktivnost(idAktivnosti, forma["naziv"], oDate, int.Parse(forma["maxBrojBodova"]));
                return RedirectToAction("Dashboard");
            }
            return RedirectToAction("prikaziGresku", new { lokacija = "edituj-aktivnost", idPoruke = 3 });
        }

        [Route("/nastavno-osoblje/{lokacija}/greska/{idPoruke}")]
        public IActionResult prikaziGresku(string lokacija, int idPoruke)
        {
            if (idPoruke == 1)
            {
                ViewBag.poruka = "Greška pri kreiranju zahtjeva";
            }
            else if (idPoruke == 2)
            {
                ViewBag.poruka = "Greška pri slanju poruke";
            }
            else if (idPoruke == 3)
            {
                ViewBag.poruka = "Greška pri editovanju zahtjeva";
            }
            return View();
        }

        [Route("/nastavno-osoblje/moji-predmeti")]
        public IActionResult mojiPredmeti()
        {
            var trenutniKorisnik = Autentifikacija.GetNastavnoOsoblje(HttpContext);
            return View(trenutniKorisnik);
        }

        [Route("/nastavno-osoblje/detalji-o-predmetu/{idPredmeta}")]
        public IActionResult detaljiOPredmetu(int idPredmeta)
        {
            var trenutniKorisnik = Autentifikacija.GetNastavnoOsoblje(HttpContext);
            ViewBag.ansambl = zmgr.dajAnsamblNaPredmetu(idPredmeta);
            ViewBag.trazeniPredmet = idPredmeta;
            return View(trenutniKorisnik);
        }

        [Route("/nastavno-osoblje/detalji-o-zadaci/{idPredmeta}/{idZadaće}")]
        [HttpGet]
        public IActionResult detaljiOZadaći(int idPredmeta, int idZadaće)
        {
            var trenutniKorisnik = Autentifikacija.GetNastavnoOsoblje(HttpContext);
            ViewBag.idZadaće = idZadaće;
            ViewBag.idPredmeta = idPredmeta;
            return View(trenutniKorisnik);
        }

        [Route("/nastavno-osoblje/ocjeni-zadaću/{idZadaće}")]
        [HttpPost]
        public IActionResult OcjeniZadaću(int idZadaće, IFormCollection forma)
        {
            NumberFormatInfo format = new NumberFormatInfo();
            format.NumberDecimalSeparator = ".";
            foreach (String key in forma.Keys)
            {
                if (!String.IsNullOrEmpty(forma[key]))
                {
                    zmgr.updateBodoveZadaćeZaStudenta(idZadaće, int.Parse(key), float.Parse(forma[key], format));
                }
            }
            return RedirectToAction("mojiPredmeti");
        }


        [Route("/nastavno-osoblje/predmet/{idPredmeta}/svi-studenti")]
        public IActionResult studentiNaPredmetu(int idPredmeta)
        {
            ViewBag.trazeniPredmet = idPredmeta;
            var trenutniKorisnik = Autentifikacija.GetNastavnoOsoblje(HttpContext);
            return View(trenutniKorisnik);
        }


    }
}