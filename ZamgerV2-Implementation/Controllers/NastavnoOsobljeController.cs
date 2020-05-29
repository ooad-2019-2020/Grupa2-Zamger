using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZamgerV2_Implementation.Models;

namespace ZamgerV2_Implementation.Controllers
{
    public class NastavnoOsobljeController : Controller
    {
        private NastavnoOsoblje trenutniKorisnik;
        private ZamgerDbContext zmgr;


        public NastavnoOsobljeController()
        {
            zmgr = ZamgerDbContext.GetInstance();
        }

        [Route("/nastavno-osoblje/dashboard/{id}")]
        public IActionResult Dashboard(int id)
        {
            KreatorKorisnika creator = new KreatorKorisnika();
            Korisnik tempK = creator.FactoryMethod(id);
            if (tempK.GetType() == typeof(NastavnoOsoblje))
            {
                trenutniKorisnik = (NastavnoOsoblje)tempK;
            }
            else
            {
                trenutniKorisnik = (Profesor)tempK;
            }
            ViewBag.listaObavjestenja = zmgr.dajSvaObavještenja();
            return View(trenutniKorisnik);
        }


        [Route("/nastavno-osoblje/{id}/kreiraj-aktivnost")]
        [HttpGet]
        public IActionResult KreirajAktivnost(int id)
        {

            KreatorKorisnika creator = new KreatorKorisnika();
            Korisnik tempK = creator.FactoryMethod(id);
            if (tempK.GetType() == typeof(NastavnoOsoblje))
            {
                trenutniKorisnik = (NastavnoOsoblje)tempK;
            }
            else
            {
                trenutniKorisnik = (Profesor)tempK;
            }

            return View(trenutniKorisnik);
        }

        [Route("/nastavno-osoblje/{id}/kreiraj-aktivnost")]
        [HttpPost]
        public IActionResult KreirajAktivnost(int id, IFormCollection forma)
        {
            KreatorKorisnika creator = new KreatorKorisnika();
            Korisnik tempK = creator.FactoryMethod(id);
            if (tempK.GetType() == typeof(NastavnoOsoblje))
            {
                trenutniKorisnik = (NastavnoOsoblje)tempK;
            }
            else
            {
                trenutniKorisnik = (Profesor)tempK;
            }

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

            return RedirectToAction("MojeAktivnosti", new { id = trenutniKorisnik.IdOsobe });
        }


        [Route("/nastavno-osoblje/{id}/moje-aktivnosti")]
        public IActionResult MojeAktivnosti(int id)
        {
            KreatorKorisnika creator = new KreatorKorisnika();
            Korisnik tempK = creator.FactoryMethod(id);
            if (tempK.GetType() == typeof(NastavnoOsoblje))
            {
                trenutniKorisnik = (NastavnoOsoblje)tempK;
            }
            else
            {
                trenutniKorisnik = (Profesor)tempK;
            }

            return View(trenutniKorisnik);
        }

        [Route("/nastavno-osoblje/osobe-list/{id}")]
        public IActionResult searchUsersForMessage(int id)
        {
            KreatorKorisnika creator = new KreatorKorisnika();
            Korisnik tempK = creator.FactoryMethod(id);
            if (tempK.GetType() == typeof(NastavnoOsoblje))
            {
                trenutniKorisnik = (NastavnoOsoblje)tempK;
            }
            else
            {
                trenutniKorisnik = (Profesor)tempK;
            }

            return View(trenutniKorisnik);
        }

        [Route("/nastavno-osoblje/osobe-list/{id}/pretraga")]
        [HttpPost]
        public IActionResult searchUsersForMessageForm(IFormCollection forma, int id)
        {
            KreatorKorisnika creator = new KreatorKorisnika();
            Korisnik tempK = creator.FactoryMethod(id);
            if (tempK.GetType() == typeof(NastavnoOsoblje))
            {
                trenutniKorisnik = (NastavnoOsoblje)tempK;
            }
            else
            {
                trenutniKorisnik = (Profesor)tempK;
            }

            //Logger logg = Logger.GetInstance();

            List<Korisnik> korisnici = zmgr.pretražiKorisnike(forma["Ime"], forma["Prezime"]);

            ViewBag.korisnici = korisnici;

            return View(trenutniKorisnik);
        }


        [Route("/nastavno-osoblje/osobe-list/{id}/pretraga/{idPrimaoca}")]
        [HttpGet]
        public IActionResult teacherSendMessage(int id, int idPrimaoca)
        {
            KreatorKorisnika creator = new KreatorKorisnika();
            Korisnik tempK = creator.FactoryMethod(id);
            if (tempK.GetType() == typeof(NastavnoOsoblje))
            {
                trenutniKorisnik = (NastavnoOsoblje)tempK;
            }
            else
            {
                trenutniKorisnik = (Profesor)tempK;
            }


            return View(trenutniKorisnik);
        }



        [Route("/nastavno-osoblje/osobe-list/{id}/pretraga/{idPrimaoca}")]
        [HttpPost]
        public IActionResult sendMessage(IFormCollection forma, int id, int idPrimaoca)
        {
            KreatorKorisnika creator = new KreatorKorisnika();
            Korisnik tempK = creator.FactoryMethod(id);
            if (tempK.GetType() == typeof(NastavnoOsoblje))
            {
                trenutniKorisnik = (NastavnoOsoblje)tempK;
            }
            else
            {
                trenutniKorisnik = (Profesor)tempK;
            }

            if (forma != null)
            {
                string sadržaj = forma["sadržaj"];
                string naslov = forma["naslov"];
                Poruka poruka = new Poruka(id, idPrimaoca, naslov, sadržaj, DateTime.Now, 0, zmgr.dajNoviPorukaId());
                zmgr.posaljiPoruku(poruka);
                //Response.WriteAsync("Poruka je poslana: " + zmgr.posaljiPoruku(poruka));
                return RedirectToAction("prikaziGresku", new { idStudenta = id, lokacija = "studenti-list/" + id + "/pretraga/" + idPrimaoca, idPoruke = 2 });
            }

            return View(trenutniKorisnik);
        }

        [Route("/nastavno-osoblje/poruke/moj-inbox/{id}")]
        public IActionResult mojInbox(int id)
        {
            KreatorKorisnika creator = new KreatorKorisnika();
            Korisnik tempK = creator.FactoryMethod(id);
            if (tempK.GetType() == typeof(NastavnoOsoblje))
            {
                trenutniKorisnik = (NastavnoOsoblje)tempK;
            }
            else
            {
                trenutniKorisnik = (Profesor)tempK;
            }
            return View(trenutniKorisnik);
        }
        [Route("/nastavno-osoblje/poruke/moj-outbox/{id}")]
        public IActionResult mojOutbox(int id)
        {
            KreatorKorisnika creator = new KreatorKorisnika();
            Korisnik tempK = creator.FactoryMethod(id);
            if (tempK.GetType() == typeof(NastavnoOsoblje))
            {
                trenutniKorisnik = (NastavnoOsoblje)tempK;
            }
            else
            {
                trenutniKorisnik = (Profesor)tempK;
            }
            return View(trenutniKorisnik);
        }

        [Route("/nastavno-osoblje/poruke/moj-inbox/{idPoruke}/{id}")]
        public IActionResult detaljiPorukeInbox(int idPoruke, int id)
        {
            KreatorKorisnika creator = new KreatorKorisnika();
            Korisnik tempK = creator.FactoryMethod(id);
            if (tempK.GetType() == typeof(NastavnoOsoblje))
            {
                trenutniKorisnik = (NastavnoOsoblje)tempK;
            }
            else
            {
                trenutniKorisnik = (Profesor)tempK;
            }
            ViewBag.poruka = zmgr.dajPoruku(idPoruke);
            zmgr.oznaciProcitanu(idPoruke);
            return View(trenutniKorisnik);
        }

        [Route("/nastavno-osoblje/poruke/moj-outbox/{idPoruke}/{id}")]
        public IActionResult detaljiPorukeOutbox(int idPoruke, int id)
        {
            KreatorKorisnika creator = new KreatorKorisnika();
            Korisnik tempK = creator.FactoryMethod(id);
            if (tempK.GetType() == typeof(NastavnoOsoblje))
            {
                trenutniKorisnik = (NastavnoOsoblje)tempK;
            }
            else
            {
                trenutniKorisnik = (Profesor)tempK;
            }
            ViewBag.poruka = zmgr.dajPoruku(idPoruke);
            return View(trenutniKorisnik);
        }



    }
}