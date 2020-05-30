using System;
using System.Collections.Generic;
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
                //Response.WriteAsync("Poruka je poslana: " + zmgr.posaljiPoruku(poruka));
                return RedirectToAction("prikaziGresku", new { lokacija = "studenti-list/pretraga/" + idPrimaoca, idPoruke = 2 });
            }
            return View(trenutniKorisnik);
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

    }
}